using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

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
	private static readonly Map<int, ScheduledFuture> _clanLocked = new();
	private static readonly Map<int, ScheduledFuture> _playerLocked = new();
	private static readonly TimeSpan LOCK_TIME = TimeSpan.FromMinutes(5);
	
	protected ClanEntryManager()
	{
		load();
	}
	
	private void load()
	{
		using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

		try 
		{
			foreach (PledgeRecruit pledgeRecruit in ctx.PledgeRecruits)
			{
				int clanId = pledgeRecruit.ClanId;

				_clanList.put(clanId,
					new PledgeRecruitInfo(clanId, pledgeRecruit.Karma, pledgeRecruit.Information,
						pledgeRecruit.DetailedInformation, pledgeRecruit.ApplicationType,
						pledgeRecruit.RecruitType));
				
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
			LOGGER.Error(GetType().Name + ": Failed to load: " + e);
		}
		
		try
		{
			var query = from waitingList in ctx.PledgeWaitingLists
				from character in ctx.Characters
				where waitingList.CharacterId == character.Id
				select new
				{
					CharacterId = character.Id,
					waitingList.Karma,
					character.Class,
					character.Level,
					CharacterName = character.Name,
				};
            
			foreach (var record in query)
			{
				_waitingList.put(record.CharacterId,
					new PledgeWaitingInfo(record.CharacterId, record.Level, record.Karma,
						record.Class, record.CharacterName));
			}
			
			LOGGER.Info(GetType().Name +": Loaded " + _waitingList.size() + " players in waiting list.");
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed to load: " + e);
		}
		
		try
		{
			var query = from applicant in ctx.PledgeApplicants
				from character in ctx.Characters
				where applicant.CharacterId == character.Id
				select new
				{
					CharacterId = character.Id,
					applicant.ClanId,
					applicant.Karma,
					applicant.Message,
					character.Level,
					CharacterName = character.Name,
				};
            
			foreach (var record in query)
			{
				_applicantList.computeIfAbsent(record.ClanId, k => new()).put(record.ClanId,
					new PledgeApplicantInfo(record.CharacterId, record.CharacterName, record.Level,
						record.Karma, record.ClanId, record.Message));
			}
			
			LOGGER.Info(GetType().Name +": Loaded " + _applicantList.size() + " player applications.");
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed to load: " + e);
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
		return _applicantList.getOrDefault(clanId, new());
	}
	
	public PledgeApplicantInfo getPlayerApplication(int clanId, int playerId)
	{
		return _applicantList.getOrDefault(clanId, new()).get(playerId);
	}
	
	public bool removePlayerApplication(int clanId, int playerId)
	{
		Map<int, PledgeApplicantInfo> clanApplicantList = _applicantList.get(clanId);
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.PledgeApplicants.Where(a => a.CharacterId == playerId && a.ClanId == clanId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
        
		return (clanApplicantList != null) && (clanApplicantList.remove(playerId) != null);
	}
	
	public bool addPlayerApplicationToClan(int clanId, PledgeApplicantInfo info)
	{
		if (!_playerLocked.containsKey(info.getPlayerId()))
		{
			_applicantList.computeIfAbsent(clanId, k => new()).put(info.getPlayerId(), info);
			
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.PledgeApplicants.Add(new PledgeApplicant()
				{
					CharacterId = info.getPlayerId(),
					ClanId = info.getRequestClanId(),
					Karma = info.getKarma(),
					Message = info.getMessage(),
				});

				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}
			
			return true;
		}
		return false;
	}
	
	public int getClanIdForPlayerApplication(int playerId)
	{
		foreach (var entry in _applicantList)
		{
			if (entry.Value.containsKey(playerId))
			{
				return entry.Key;
			}
		}
		return 0;
	}
	
	public bool addToWaitingList(int playerId, PledgeWaitingInfo info)
	{
		if (!_playerLocked.containsKey(playerId))
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.PledgeWaitingLists.Add(new PledgeWaitingList
				{
					CharacterId = info.getPlayerId(),
					Karma = info.getKarma()
				});

				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
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
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.PledgeWaitingLists.Where(x => x.CharacterId == playerId).ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
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
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.PledgeRecruits.Add(new PledgeRecruit()
				{
					ClanId = info.getClanId(),
					Karma = info.getKarma(),
					Information = info.getInformation(),
					DetailedInformation = info.getDetailedInformation(),
					ApplicationType = info.getApplicationType(),
					RecruitType = info.getRecruitType(),
				});

				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
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
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.PledgeRecruits.Where(r => r.ClanId == clanId)
					.ExecuteUpdate(s =>
						s.SetProperty(r => r.Karma, info.getKarma())
							.SetProperty(r => r.Information, info.getInformation())
							.SetProperty(r => r.DetailedInformation, info.getDetailedInformation())
							.SetProperty(r => r.ApplicationType, info.getApplicationType())
							.SetProperty(r => r.RecruitType, info.getRecruitType()));
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}

			bool result = _clanList.ContainsKey(clanId);
			_clanList[clanId] = info;
			return result;
		}
        
		return false;
	}
	
	public bool removeFromClanList(int clanId)
	{
		if (_clanList.containsKey(clanId))
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.PledgeRecruits.Where(r => r.ClanId == clanId).ExecuteDelete();
			}
			catch (Exception e)
			{
				LOGGER.Error(e);
			}
			
			_clanList.remove(clanId);
			lockClan(clanId);
			return true;
		}
		return false;
	}
	
	public List<PledgeWaitingInfo> getSortedWaitingList(int levelMin, int levelMax, int role, int sortByValue, bool descending)
	{
		List<PledgeWaitingInfo> result = new();
		foreach (PledgeWaitingInfo p in _waitingList.values())
		{
			// TODO: Handle Role.
			if ((p.getPlayerLvl() >= levelMin) && (p.getPlayerLvl() <= levelMax))
			{
				result.Add(p);
			}
		}

		int sortBy = Math.Clamp(sortByValue, 1, 4);
		Comparison<PledgeWaitingInfo> comparison;
		switch (sortBy)
		{
			case 1:
				comparison = (a, b) => string.CompareOrdinal(a.getPlayerName(), b.getPlayerName());
				break;
			case 2:
				comparison = (a, b) => a.getKarma().CompareTo(b.getKarma());
				break;
			case 3:
				comparison = (a, b) => a.getPlayerLvl().CompareTo(b.getPlayerLvl());
				break;
			default:
				comparison = (a, b) => a.getPlayerClassId().CompareTo(b.getPlayerClassId());
				break;
		}

		Comparison<PledgeWaitingInfo> finalComparison;
		if (descending)
			finalComparison = (a, b) => -comparison(a, b);
		else
			finalComparison = comparison;
		
		result.Sort(finalComparison);
		return result;
	}
	
	public List<PledgeWaitingInfo> queryWaitingListByName(String name)
	{
		List<PledgeWaitingInfo> result = new();
		foreach (PledgeWaitingInfo p in _waitingList.values())
		{
			if (p.getPlayerName().toLowerCase().contains(name))
			{
				result.Add(p);
			}
		}
		return result;
	}
	
	public List<PledgeRecruitInfo> getSortedClanListByName(String query, int type)
	{
		List<PledgeRecruitInfo> result = new();
		if (type == 1)
		{
			foreach (PledgeRecruitInfo p in _clanList.values())
			{
				if (p.getClanName().toLowerCase().contains(query))
				{
					result.Add(p);
				}
			}
		}
		else
		{
			foreach (PledgeRecruitInfo p in _clanList.values())
			{
				if (p.getClanLeaderName().toLowerCase().contains(query))
				{
					result.Add(p);
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
		return _clanList.values().ToList();
	}
	
	public List<PledgeRecruitInfo> getSortedClanList(int clanLevel, int karma, int sortByValue, bool descending)
	{
		List<PledgeRecruitInfo> sortedList = new();
		for (int i = 0; i < sortedList.Count; i++)
		{
			PledgeRecruitInfo currentInfo = sortedList[i];
			if (((clanLevel < 0) && (karma >= 0) && (karma != currentInfo.getKarma())) || ((clanLevel >= 0) && (karma < 0) && (clanLevel != (currentInfo.getClan() != null ? currentInfo.getClanLevel() : 0))) || ((clanLevel >= 0) && (karma >= 0) && ((clanLevel != (currentInfo.getClan() != null ? currentInfo.getClanLevel() : 0)) || (karma != currentInfo.getKarma()))))
			{
				sortedList.RemoveAt(i--);
			}
		}
		
		int sortBy = Math.Clamp(sortByValue, 1, 4);
		Comparison<PledgeRecruitInfo> comparison;
		switch (sortBy)
		{
			case 1:
				comparison = (a, b) => string.CompareOrdinal(a.getClanName(), b.getClanName());
				break;
			case 2:
				comparison = (a, b) => string.CompareOrdinal(a.getClanLeaderName(), b.getClanLeaderName());
				break;
			case 3:
				comparison = (a, b) => a.getClanLevel().CompareTo(b.getClanLevel());
				break;
			default:
				comparison = (a, b) => a.getKarma().CompareTo(b.getKarma());
				break;
		}
        
		Comparison<PledgeRecruitInfo> finalComparison;
		if (descending)
			finalComparison = (a, b) => -comparison(a, b);
		else
			finalComparison = comparison;
		
		sortedList.Sort(finalComparison);
		return sortedList;
	}
	
	public TimeSpan getPlayerLockTime(int playerId)
	{
		return _playerLocked.get(playerId) == null ? TimeSpan.Zero : _playerLocked.get(playerId).getDelay();
	}
	
	public TimeSpan getClanLockTime(int playerId)
	{
		return _clanLocked.get(playerId) == null ? TimeSpan.Zero : _clanLocked.get(playerId).getDelay();
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
		public static readonly ClanEntryManager INSTANCE = new ClanEntryManager();
	}
}