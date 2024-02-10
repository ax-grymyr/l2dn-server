using System.Runtime.CompilerServices;
using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Clans;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Data.Sql;

/**
 * This class loads the clan related data.
 */
public class ClanTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanTable));
	private readonly Map<int, Clan> _clans = new();
	
	protected ClanTable()
	{
		// forums has to be loaded before clan data, because of last forum id used should have also memo included
		if (Config.ENABLE_COMMUNITY_BOARD)
		{
			ForumsBBSManager.getInstance().initRoot();
		}
		
		// Get all clan ids.
		List<int> cids = new();
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT clan_id FROM clan_data");
			while (rs.next())
			{
				cids.add(rs.getInt("clan_id"));
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error restoring ClanTable.", e);
		}
		
		// Create clans.
		foreach (int cid in cids)
		{
			Clan clan = new Clan(cid);
			_clans.put(cid, clan);
			if (clan.getDissolvingExpiryTime() != 0)
			{
				scheduleRemoveClan(clan.getId());
			}
		}
		
		LOGGER.Info(GetType().Name + ": Restored " + cids.size() + " clans from the database.");
		allianceCheck();
		restoreClanWars();
	}
	
	/**
	 * Gets the clans.
	 * @return the clans
	 */
	public ICollection<Clan> getClans()
	{
		return _clans.values();
	}
	
	/**
	 * Gets the clan count.
	 * @return the clan count
	 */
	public int getClanCount()
	{
		return _clans.size();
	}
	
	/**
	 * @param clanId
	 * @return
	 */
	public Clan getClan(int clanId)
	{
		return _clans.get(clanId);
	}
	
	public Clan getClanByName(String clanName)
	{
		foreach (Clan clan in _clans.values())
		{
			if (clan.getName().equalsIgnoreCase(clanName))
			{
				return clan;
			}
		}
		return null;
	}
	
	/**
	 * Creates a new clan and store clan info to database
	 * @param player
	 * @param clanName
	 * @return NULL if clan with same name already exists
	 */
	public Clan createClan(Player player, String clanName)
	{
		if (player == null)
		{
			return null;
		}
		
		// if (player.getLevel() < 10)
		// {
		// player.sendPacket(SystemMessageId.YOU_DO_NOT_MEET_THE_CRITERIA_IN_ORDER_TO_CREATE_A_CLAN);
		// return null;
		// }
		if (player.getClanId() != 0)
		{
			player.sendPacket(SystemMessageId.FAILED_TO_CREATE_A_CLAN);
			return null;
		}
		if (System.currentTimeMillis() < player.getClanCreateExpiryTime())
		{
			player.sendPacket(SystemMessageId.YOU_MUST_WAIT_10_DAYS_BEFORE_CREATING_A_NEW_CLAN);
			return null;
		}
		if (!Util.isAlphaNumeric(clanName) || (clanName.Length < 2))
		{
			player.sendPacket(SystemMessageId.CLAN_NAME_IS_INVALID);
			return null;
		}
		if (clanName.Length > 16)
		{
			player.sendPacket(SystemMessageId.CLAN_NAME_S_LENGTH_IS_INCORRECT);
			return null;
		}
		
		if (getClanByName(clanName) != null)
		{
			// clan name is already taken
			SystemMessage sm = new SystemMessage(SystemMessageId.S1_ALREADY_EXISTS);
			sm.addString(clanName);
			player.sendPacket(sm);
			return null;
		}
		
		Clan clan = new Clan(IdManager.getInstance().getNextId(), clanName);
		ClanMember leader = new ClanMember(clan, player);
		clan.setLeader(leader);
		leader.setPlayer(player);
		clan.store();
		player.setClan(clan);
		player.setPledgeClass(ClanMember.calculatePledgeClass(player));
		player.setClanPrivileges(new EnumIntBitmask<>(ClanPrivilege, true));
		
		_clans.put(clan.getId(), clan);
		
		// should be update packet only
		player.sendPacket(new PledgeShowInfoUpdate(clan));
		PledgeShowMemberListAll.sendAllTo(player);
		player.sendPacket(new PledgeShowMemberListUpdate(player));
		player.sendPacket(SystemMessageId.YOUR_CLAN_HAS_BEEN_CREATED);
		player.broadcastUserInfo(UserInfoType.RELATION, UserInfoType.CLAN);
		
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_CLAN_CREATE))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerClanCreate(player, clan));
		}
		
		return clan;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void destroyClan(int clanId)
	{
		Clan clan = getClan(clanId);
		if (clan == null)
		{
			return;
		}
		
		clan.broadcastToOnlineMembers(new SystemMessage(SystemMessageId.CLAN_HAS_DISPERSED));
		
		ClanEntryManager.getInstance().removeFromClanList(clan.getId());
		
		int castleId = clan.getCastleId();
		if (castleId == 0)
		{
			foreach (Siege siege in SiegeManager.getInstance().getSieges())
			{
				siege.removeSiegeClan(clan);
			}
		}
		
		int fortId = clan.getFortId();
		if (fortId == 0)
		{
			foreach (FortSiege siege in FortSiegeManager.getInstance().getSieges())
			{
				siege.removeAttacker(clan);
			}
		}
		
		ClanHall hall = ClanHallData.getInstance().getClanHallByClan(clan);
		if (hall != null)
		{
			hall.setOwner(null);
		}
		
		ClanMember leaderMember = clan.getLeader();
		if (leaderMember == null)
		{
			clan.getWarehouse().destroyAllItems("ClanRemove", null, null);
		}
		else
		{
			clan.getWarehouse().destroyAllItems("ClanRemove", clan.getLeader().getPlayer(), null);
		}
		
		foreach (ClanMember member in clan.getMembers())
		{
			clan.removeClanMember(member.getObjectId(), 0);
		}
		
		_clans.remove(clanId);
		IdManager.getInstance().releaseId(clanId);
		
		try
		{
			Connection con = DatabaseFactory.getConnection();

			{
				PreparedStatement ps = con.prepareStatement("DELETE FROM clan_data WHERE clan_id=?");
				ps.setInt(1, clanId);
				ps.execute();
			}


			{
				PreparedStatement ps = con.prepareStatement("DELETE FROM clan_privs WHERE clan_id=?");
				ps.setInt(1, clanId);
				ps.execute();
			}


			{
				PreparedStatement ps = con.prepareStatement("DELETE FROM clan_skills WHERE clan_id=?");
				ps.setInt(1, clanId);
				ps.execute();
			}

			{
				PreparedStatement ps = con.prepareStatement("DELETE FROM clan_subpledges WHERE clan_id=?");
				ps.setInt(1, clanId);
				ps.execute();
			}


			{
				PreparedStatement ps = con.prepareStatement("DELETE FROM clan_wars WHERE clan1=? OR clan2=?");
				ps.setInt(1, clanId);
				ps.setInt(2, clanId);
				ps.execute();
			}


			{
				PreparedStatement ps = con.prepareStatement("DELETE FROM clan_notices WHERE clan_id=?");
				ps.setInt(1, clanId);
				ps.execute();
			}
			
			if (fortId != 0)
			{
				Fort fort = FortManager.getInstance().getFortById(fortId);
				if (fort != null)
				{
					Clan owner = fort.getOwnerClan();
					if (clan == owner)
					{
						fort.removeOwner(true);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error removing clan from DB.", e);
		}
		
		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_CLAN_DESTROY))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnPlayerClanDestroy(leaderMember, clan));
		}
	}
	
	public void scheduleRemoveClan(int clanId)
	{
		ThreadPool.schedule(() =>
		{
			if (getClan(clanId) == null)
			{
				return;
			}
			if (getClan(clanId).getDissolvingExpiryTime() != 0)
			{
				destroyClan(clanId);
			}
		}, Math.Max(getClan(clanId).getDissolvingExpiryTime() - System.currentTimeMillis(), 300000));
	}
	
	public bool isAllyExists(String allyName)
	{
		foreach (Clan clan in _clans.values())
		{
			if ((clan.getAllyName() != null) && clan.getAllyName().equalsIgnoreCase(allyName))
			{
				return true;
			}
		}
		return false;
	}
	
	public void storeClanWars(ClanWar war)
	{
		try
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"REPLACE INTO clan_wars (clan1, clan2, clan1Kill, clan2Kill, winnerClan, startTime, endTime, state) VALUES(?,?,?,?,?,?,?,?)");
			ps.setInt(1, war.getAttackerClanId());
			ps.setInt(2, war.getAttackedClanId());
			ps.setInt(3, war.getAttackerKillCount());
			ps.setInt(4, war.getAttackedKillCount());
			ps.setInt(5, war.getWinnerClanId());
			ps.setLong(6, war.getStartTime());
			ps.setLong(7, war.getEndTime());
			ps.setInt(8, war.getState().ordinal());
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error storing clan wars data: " + e);
		}
	}
	
	public void deleteClanWars(int clanId1, int clanId2)
	{
		Clan clan1 = getInstance().getClan(clanId1);
		Clan clan2 = getInstance().getClan(clanId2);
		
		if (EventDispatcher.getInstance().hasListener(EventType.ON_CLAN_WAR_FINISH))
		{
			EventDispatcher.getInstance().notifyEventAsync(new OnClanWarFinish(clan1, clan2));
		}
		
		clan1.deleteWar(clan2.getId());
		clan2.deleteWar(clan1.getId());
		clan1.broadcastClanStatus();
		clan2.broadcastClanStatus();
		
		try
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("DELETE FROM clan_wars WHERE clan1=? AND clan2=?");
			ps.setInt(1, clanId1);
			ps.setInt(2, clanId2);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error removing clan wars data.", e);
		}
	}
	
	private void restoreClanWars()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement statement = con.createStatement();
			ResultSet rset = statement.executeQuery(
				"SELECT clan1, clan2, clan1Kill, clan2Kill, winnerClan, startTime, endTime, state FROM clan_wars");
			while (rset.next())
			{
				Clan attacker = getClan(rset.getInt("clan1"));
				Clan attacked = getClan(rset.getInt("clan2"));
				if ((attacker != null) && (attacked != null))
				{
					ClanWarState state = ClanWarState.values()[rset.getInt("state")];
					ClanWar clanWar = new ClanWar(attacker, attacked, rset.getInt("clan1Kill"), rset.getInt("clan2Kill"), rset.getInt("winnerClan"), rset.getLong("startTime"), rset.getLong("endTime"), state);
					attacker.addWar(attacked.getId(), clanWar);
					attacked.addWar(attacker.getId(), clanWar);
				}
				else
				{
					LOGGER.Warn(GetType().Name + ": Restorewars one of clans is null attacker:" + attacker + " attacked:" + attacked);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error restoring clan wars data.", e);
		}
	}
	
	/**
	 * Check for nonexistent alliances
	 */
	private void allianceCheck()
	{
		foreach (Clan clan in _clans.values())
		{
			int allyId = clan.getAllyId();
			if ((allyId != 0) && (clan.getId() != allyId) && !_clans.containsKey(allyId))
			{
				clan.setAllyId(0);
				clan.setAllyName(null);
				clan.changeAllyCrest(0, true);
				clan.updateClanInDB();
				LOGGER.Info(GetType().Name + ": Removed alliance from clan: " + clan);
			}
		}
	}
	
	public List<Clan> getClanAllies(int allianceId)
	{
		List<Clan> clanAllies = new();
		if (allianceId != 0)
		{
			foreach (Clan clan in _clans.values())
			{
				if ((clan != null) && (clan.getAllyId() == allianceId))
				{
					clanAllies.add(clan);
				}
			}
		}
		return clanAllies;
	}
	
	public void shutdown()
	{
		foreach (Clan clan in _clans.values())
		{
			clan.updateClanInDB();
			clan.getVariables().storeMe();
			foreach (ClanWar war in clan.getWarList().values())
			{
				storeClanWars(war);
			}
		}
	}
	
	public static ClanTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ClanTable INSTANCE = new ClanTable();
	}
}