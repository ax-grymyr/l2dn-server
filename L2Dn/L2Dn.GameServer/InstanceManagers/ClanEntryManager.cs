using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Sdw
 */
public class ClanEntryManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanEntryManager));
	
	private static readonly Map<int, PledgeWaitingInfo> _waitingList = new();
	private static readonly Map<int, PledgeRecruitInfo> _clanList = new();
	private static readonly Map<int, Map<int, PledgeApplicantInfo>> _applicantList = new();
	
	private static readonly Map<int, ScheduledFuture<?>> _clanLocked = new();
	private static readonly Map<int, ScheduledFuture<?>> _playerLocked = new();
	
	private const string INSERT_APPLICANT = "REPLACE INTO pledge_applicant VALUES (?, ?, ?, ?)";
	private const string DELETE_APPLICANT = "DELETE FROM pledge_applicant WHERE charId = ? AND clanId = ?";
	
	private const string INSERT_WAITING_LIST = "INSERT INTO pledge_waiting_list VALUES (?, ?)";
	private const string DELETE_WAITING_LIST = "DELETE FROM pledge_waiting_list WHERE char_id = ?";
	
	private const string INSERT_CLAN_RECRUIT = "INSERT INTO pledge_recruit VALUES (?, ?, ?, ?, ?, ?)";
	private const string UPDATE_CLAN_RECRUIT = "UPDATE pledge_recruit SET karma = ?, information = ?, detailed_information = ?, application_type = ?, recruit_type = ? WHERE clan_id = ?";
	private const string DELETE_CLAN_RECRUIT = "DELETE FROM pledge_recruit WHERE clan_id = ?";
	
	//@formatter:off
	private static readonly List<Comparator<PledgeWaitingInfo>> PLAYER_COMPARATOR = Arrays.asList(
		null,
		Comparator.comparing(PledgeWaitingInfo::getPlayerName), 
		Comparator.comparingInt(PledgeWaitingInfo::getKarma), 
		Comparator.comparingInt(PledgeWaitingInfo::getPlayerLvl), 
		Comparator.comparingInt(PledgeWaitingInfo::getPlayerClassId));
	//@formatter:on
	
	//@formatter:off
	private static readonly List<Comparator<PledgeRecruitInfo>> CLAN_COMPARATOR = Arrays.asList(
		null,
		Comparator.comparing(PledgeRecruitInfo::getClanName),
		Comparator.comparing(PledgeRecruitInfo::getClanLeaderName),
		Comparator.comparingInt(PledgeRecruitInfo::getClanLevel),
		Comparator.comparingInt(PledgeRecruitInfo::getKarma));
	//@formatter:on
	
	private static readonly long LOCK_TIME = TimeUnit.MINUTES.toMillis(5);
	
	protected ClanEntryManager()
	{
		load();
	}
	
	private void load()
	{
		try (using GameServerDbContext ctx = new();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT * FROM pledge_recruit"))
		{
			while (rs.next())
			{
				int clanId = rs.getInt("clan_id");
				_clanList.put(clanId, new PledgeRecruitInfo(clanId, rs.getInt("karma"), rs.getString("information"), rs.getString("detailed_information"), rs.getInt("application_type"), rs.getInt("recruit_type")));
				// Remove non existing clan data.
				if (ClanTable.getInstance().getClan(clanId) == null)
				{
					removeFromClanList(clanId);
				}
			}
			LOGGER.Info(GetType().Name +": Loaded " + _clanList.size() + " clan entries.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed to load: " + e);
		}
		
		try (using GameServerDbContext ctx = new();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT a.char_id, a.karma, b.base_class, b.level, b.char_name FROM pledge_waiting_list as a LEFT JOIN characters as b ON a.char_id = b.charId"))
		{
			while (rs.next())
			{
				_waitingList.put(rs.getInt("char_id"), new PledgeWaitingInfo(rs.getInt("char_id"), rs.getInt("level"), rs.getInt("karma"), rs.getInt("base_class"), rs.getString("char_name")));
			}
			
			LOGGER.Info(GetType().Name +": Loaded " + _waitingList.size() + " players in waiting list.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed to load: " + e);
		}
		
		try (using GameServerDbContext ctx = new();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT a.charId, a.clanId, a.karma, a.message, b.base_class, b.level, b.char_name FROM pledge_applicant as a LEFT JOIN characters as b ON a.charId = b.charId"))
		{
			while (rs.next())
			{
				_applicantList.computeIfAbsent(rs.getInt("clanId"), k => new ConcurrentHashMap<>()).put(rs.getInt("charId"), new PledgeApplicantInfo(rs.getInt("charId"), rs.getString("char_name"), rs.getInt("level"), rs.getInt("karma"), rs.getInt("clanId"), rs.getString("message")));
			}
			
			LOGGER.Info(GetType().Name +": Loaded " + _applicantList.size() + " player applications.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Failed to load: " + e);
		}
	}
	
	public Map<int, PledgeWaitingInfo> getWaitingList()
	{
		return _waitingList;
	}
	
	public Map<int, PledgeRecruitInfo> getClanList()
	{
		return _clanList;
	}
	
	public Map<int, Map<int, PledgeApplicantInfo>> getApplicantList()
	{
		return _applicantList;
	}
	
	public Map<int, PledgeApplicantInfo> getApplicantListForClan(int clanId)
	{
		return _applicantList.getOrDefault(clanId, Collections.emptyMap());
	}
	
	public PledgeApplicantInfo getPlayerApplication(int clanId, int playerId)
	{
		return _applicantList.getOrDefault(clanId, Collections.emptyMap()).get(playerId);
	}
	
	public bool removePlayerApplication(int clanId, int playerId)
	{
		Map<int, PledgeApplicantInfo> clanApplicantList = _applicantList.get(clanId);
		
		try (using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(DELETE_APPLICANT))
		{
			statement.setInt(1, playerId);
			statement.setInt(2, clanId);
			statement.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e.getMessage(), e);
		}
		return (clanApplicantList != null) && (clanApplicantList.remove(playerId) != null);
	}
	
	public bool addPlayerApplicationToClan(int clanId, PledgeApplicantInfo info)
	{
		if (!_playerLocked.containsKey(info.getPlayerId()))
		{
			_applicantList.computeIfAbsent(clanId, k => new ConcurrentHashMap<>()).put(info.getPlayerId(), info);
			
			try (using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(INSERT_APPLICANT))
			{
				statement.setInt(1, info.getPlayerId());
				statement.setInt(2, info.getRequestClanId());
				statement.setInt(3, info.getKarma());
				statement.setString(4, info.getMessage());
				statement.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(e.getMessage(), e);
			}
			return true;
		}
		return false;
	}
	
	public int getClanIdForPlayerApplication(int playerId)
	{
		for (Entry<int, Map<int, PledgeApplicantInfo>> entry : _applicantList.entrySet())
		{
			if (entry.getValue().containsKey(playerId))
			{
				return entry.getKey();
			}
		}
		return 0;
	}
	
	public bool addToWaitingList(int playerId, PledgeWaitingInfo info)
	{
		if (!_playerLocked.containsKey(playerId))
		{
			try (using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(INSERT_WAITING_LIST))
			{
				statement.setInt(1, info.getPlayerId());
				statement.setInt(2, info.getKarma());
				statement.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(e.getMessage(), e);
			}
			_waitingList.put(playerId, info);
			return true;
		}
		return false;
	}
	
	public bool removeFromWaitingList(int playerId)
	{
		if (_waitingList.containsKey(playerId))
		{
			try (using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(DELETE_WAITING_LIST))
			{
				statement.setInt(1, playerId);
				statement.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(e.getMessage(), e);
			}
			_waitingList.remove(playerId);
			lockPlayer(playerId);
			return true;
		}
		return false;
	}
	
	public bool addToClanList(int clanId, PledgeRecruitInfo info)
	{
		if (!_clanList.containsKey(clanId) && !_clanLocked.containsKey(clanId))
		{
			try (using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(INSERT_CLAN_RECRUIT))
			{
				statement.setInt(1, info.getClanId());
				statement.setInt(2, info.getKarma());
				statement.setString(3, info.getInformation());
				statement.setString(4, info.getDetailedInformation());
				statement.setInt(5, info.getApplicationType());
				statement.setInt(6, info.getRecruitType());
				statement.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(e.getMessage(), e);
			}
			_clanList.put(clanId, info);
			return true;
		}
		return false;
	}
	
	public bool updateClanList(int clanId, PledgeRecruitInfo info)
	{
		if (_clanList.containsKey(clanId) && !_clanLocked.containsKey(clanId))
		{
			try (using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(UPDATE_CLAN_RECRUIT))
			{
				statement.setInt(1, info.getKarma());
				statement.setString(2, info.getInformation());
				statement.setString(3, info.getDetailedInformation());
				statement.setInt(4, info.getApplicationType());
				statement.setInt(5, info.getRecruitType());
				statement.setInt(6, info.getClanId());
				statement.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(e.getMessage(), e);
			}
			return _clanList.replace(clanId, info) != null;
		}
		return false;
	}
	
	public bool removeFromClanList(int clanId)
	{
		if (_clanList.containsKey(clanId))
		{
			try (using GameServerDbContext ctx = new();
				PreparedStatement statement = con.prepareStatement(DELETE_CLAN_RECRUIT))
			{
				statement.setInt(1, clanId);
				statement.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(e.getMessage(), e);
			}
			_clanList.remove(clanId);
			lockClan(clanId);
			return true;
		}
		return false;
	}
	
	public List<PledgeWaitingInfo> getSortedWaitingList(int levelMin, int levelMax, int role, int sortByValue, bool descending)
	{
		int sortBy = CommonUtil.constrain(sortByValue, 1, PLAYER_COMPARATOR.size() - 1);
		List<PledgeWaitingInfo> result = new();
		for (PledgeWaitingInfo p : _waitingList.values())
		{
			// TODO: Handle Role.
			if ((p.getPlayerLvl() >= levelMin) && (p.getPlayerLvl() <= levelMax))
			{
				result.add(p);
			}
		}
		result.sort(descending ? PLAYER_COMPARATOR.get(sortBy).reversed() : PLAYER_COMPARATOR.get(sortBy));
		return result;
	}
	
	public List<PledgeWaitingInfo> queryWaitingListByName(String name)
	{
		List<PledgeWaitingInfo> result = new();
		for (PledgeWaitingInfo p : _waitingList.values())
		{
			if (p.getPlayerName().toLowerCase().contains(name))
			{
				result.add(p);
			}
		}
		return result;
	}
	
	public List<PledgeRecruitInfo> getSortedClanListByName(String query, int type)
	{
		List<PledgeRecruitInfo> result = new();
		if (type == 1)
		{
			for (PledgeRecruitInfo p : _clanList.values())
			{
				if (p.getClanName().toLowerCase().contains(query))
				{
					result.add(p);
				}
			}
		}
		else
		{
			for (PledgeRecruitInfo p : _clanList.values())
			{
				if (p.getClanLeaderName().toLowerCase().contains(query))
				{
					result.add(p);
				}
			}
		}
		return result;
	}
	
	public PledgeRecruitInfo getClanById(int clanId)
	{
		return _clanList.get(clanId);
	}
	
	public bool isClanRegistred(int clanId)
	{
		return _clanList.get(clanId) != null;
	}
	
	public bool isPlayerRegistred(int playerId)
	{
		return _waitingList.get(playerId) != null;
	}
	
	public List<PledgeRecruitInfo> getUnSortedClanList()
	{
		return new ArrayList<>(_clanList.values());
	}
	
	public List<PledgeRecruitInfo> getSortedClanList(int clanLevel, int karma, int sortByValue, bool descending)
	{
		int sortBy = CommonUtil.constrain(sortByValue, 1, CLAN_COMPARATOR.size() - 1);
		List<PledgeRecruitInfo> sortedList = new ArrayList<>(_clanList.values());
		for (int i = 0; i < sortedList.size(); i++)
		{
			PledgeRecruitInfo currentInfo = sortedList.get(i);
			if (((clanLevel < 0) && (karma >= 0) && (karma != currentInfo.getKarma())) || ((clanLevel >= 0) && (karma < 0) && (clanLevel != (currentInfo.getClan() != null ? currentInfo.getClanLevel() : 0))) || ((clanLevel >= 0) && (karma >= 0) && ((clanLevel != (currentInfo.getClan() != null ? currentInfo.getClanLevel() : 0)) || (karma != currentInfo.getKarma()))))
			{
				sortedList.remove(i--);
			}
		}
		Collections.sort(sortedList, descending ? CLAN_COMPARATOR.get(sortBy).reversed() : CLAN_COMPARATOR.get(sortBy));
		return sortedList;
	}
	
	public long getPlayerLockTime(int playerId)
	{
		return _playerLocked.get(playerId) == null ? 0 : _playerLocked.get(playerId).getDelay(TimeUnit.MINUTES);
	}
	
	public long getClanLockTime(int playerId)
	{
		return _clanLocked.get(playerId) == null ? 0 : _clanLocked.get(playerId).getDelay(TimeUnit.MINUTES);
	}
	
	private void lockPlayer(int playerId)
	{
		_playerLocked.put(playerId, ThreadPool.schedule(() => _playerLocked.remove(playerId), LOCK_TIME));
	}
	
	private void lockClan(int clanId)
	{
		_clanLocked.put(clanId, ThreadPool.schedule(() => _clanLocked.remove(clanId), LOCK_TIME));
	}
	
	public static ClanEntryManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		protected static readonly ClanEntryManager INSTANCE = new ClanEntryManager();
	}
}
