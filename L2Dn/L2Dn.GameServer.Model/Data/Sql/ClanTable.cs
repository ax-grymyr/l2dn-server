using System.Runtime.CompilerServices;
using L2Dn.GameServer.CommunityBbs.Managers;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Clans;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using ClanWar = L2Dn.GameServer.Model.Clans.ClanWar;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

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
		List<int> cids;
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			cids = ctx.Clans.Select(c => c.Id).ToList();
		}
		catch (Exception e)
		{
			LOGGER.Error("Error restoring ClanTable.", e);
			cids = new();
		}

		// Create clans.
		foreach (int cid in cids)
		{
			Clan clan = new Clan(cid);
			_clans.put(cid, clan);
			if (clan.getDissolvingExpiryTime() is not null)
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
	public Clan? getClan(int clanId)
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
	public Clan? createClan(Player player, String clanName)
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
		if (DateTime.UtcNow < player.getClanCreateExpiryTime())
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
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_ALREADY_EXISTS);
			sm.Params.addString(clanName);
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
		player.setClanPrivileges(ClanPrivilege.All);
		
		_clans.put(clan.getId(), clan);
		
		// should be update packet only
		player.sendPacket(new PledgeShowInfoUpdatePacket(clan));
		PledgeShowMemberListAllPacket.sendAllTo(player);
		player.sendPacket(new PledgeShowMemberListUpdatePacket(player));
		player.sendPacket(SystemMessageId.YOUR_CLAN_HAS_BEEN_CREATED);
		player.broadcastUserInfo(UserInfoType.RELATION, UserInfoType.CLAN);
		
		// Notify to scripts
		if (GlobalEvents.Global.HasSubscribers<OnClanCreate>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanCreate(player, clan));
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
		
		clan.broadcastToOnlineMembers(new SystemMessagePacket(SystemMessageId.CLAN_HAS_DISPERSED));
		
		ClanEntryManager.getInstance().removeFromClanList(clan.getId());
		
		int? castleId = clan.getCastleId();
		if (castleId == 0)
		{
			foreach (Siege siege in SiegeManager.getInstance().getSieges())
			{
				siege.removeSiegeClan(clan);
			}
		}
		
		int? fortId = clan.getFortId();
		if (fortId == null || fortId == 0)
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
			clan.removeClanMember(member.getObjectId(), null);
		}
		
		_clans.remove(clanId);
		IdManager.getInstance().releaseId(clanId);
		
		try
		{
			// TODO: set cascade delete for dependent tables
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.Clans.Where(c => c.Id == clanId).ExecuteDelete();
			ctx.ClanPrivileges.Where(c => c.ClanId == clanId).ExecuteDelete();
			ctx.ClanSkills.Where(c => c.ClanId == clanId).ExecuteDelete();
			ctx.ClanSubPledges.Where(c => c.ClanId == clanId).ExecuteDelete();
			ctx.ClanWars.Where(c => c.Clan1Id == clanId || c.Clan2Id == clanId).ExecuteDelete();
			ctx.ClanNotices.Where(c => c.ClanId == clanId).ExecuteDelete();
			
			if (fortId != null && fortId != 0)
			{
				Fort fort = FortManager.getInstance().getFortById(fortId.Value);
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
		if (GlobalEvents.Global.HasSubscribers<OnClanDestroy>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanDestroy(leaderMember, clan));
		}
	}
	
	public void scheduleRemoveClan(int clanId)
	{
		TimeSpan delay = getClan(clanId).getDissolvingExpiryTime().Value - DateTime.UtcNow;
		if (delay < TimeSpan.FromMilliseconds(300000))
			delay = TimeSpan.FromMilliseconds(300000);
			
		ThreadPool.schedule(() =>
		{
			if (getClan(clanId) == null)
			{
				return;
			}
			if (getClan(clanId).getDissolvingExpiryTime() != DateTime.MinValue)
			{
				destroyClan(clanId);
			}
		}, delay);
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var dbWar = new Db.ClanWar
			{
				Clan1Id = war.getAttackerClanId(),
				Clan2Id = war.getAttackedClanId(),
				Clan1Kills = war.getAttackerKillCount(),
				Clan2Kills = war.getAttackedKillCount(),
				WinnerClanId = war.getWinnerClanId(),
				StartTime = war.getStartTime(),
				EndTime = war.getEndTime(),
				State = (short)war.getState()
			};
			
			ctx.ClanWars.Add(dbWar);
			ctx.SaveChanges();
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
		
		if (GlobalEvents.Global.HasSubscribers<OnClanWarFinish>())
		{
			GlobalEvents.Global.NotifyAsync(new OnClanWarFinish(clan1, clan2));
		}
		
		clan1.deleteWar(clan2.getId());
		clan2.deleteWar(clan1.getId());
		clan1.broadcastClanStatus();
		clan2.broadcastClanStatus();
		
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ClanWars.Where(cw => cw.Clan1Id == clanId1 && cw.Clan2Id == clanId2).ExecuteDelete();
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var war in ctx.ClanWars)
			{
				Clan attacker = getClan(war.Clan1Id);
				Clan attacked = getClan(war.Clan2Id);
				if ((attacker != null) && (attacked != null))
				{
					ClanWarState state = (ClanWarState)war.State;
					ClanWar clanWar = new ClanWar(attacker, attacked, war.Clan1Kills, war.Clan2Kills,
						war.WinnerClanId, war.StartTime, war.EndTime, state);
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
			int? allyId = clan.getAllyId();
			if ((allyId != null) && (clan.getId() != allyId) && !_clans.containsKey(allyId.Value))
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