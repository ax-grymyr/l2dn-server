using L2Dn.Events;
using L2Dn.Extensions;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events.Impl.Sieges;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Sieges;

public class Siege: Siegable
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(Siege));
	
	// typeId's
	public const sbyte OWNER = -1;
	public const byte DEFENDER = 0;
	public const byte ATTACKER = 1;
	public const byte DEFENDER_NOT_APPROVED = 2;
	
	private int _controlTowerCount;
	
	// must support Concurrent Modifications
	private readonly Set<SiegeClan> _attackerClans = new();
	private readonly Set<SiegeClan> _defenderClans = new();
	private readonly Set<SiegeClan> _defenderWaitingClans = new();
	
	// Castle setting
	private readonly List<ControlTower> _controlTowers = new();
	private readonly List<FlameTower> _flameTowers = new();
	private readonly Castle _castle;
	private bool _isInProgress = false;
	private bool _isNormalSide = true; // true = Atk is Atk, false = Atk is Def
	protected bool _isRegistrationOver = false;
	protected DateTime _siegeEndDate;
	protected ScheduledFuture _scheduledStartSiegeTask = null;
	protected ScheduledFuture _scheduledSiegeInfoTask = null;
	protected int _firstOwnerClanId = -1;
	
	public Siege(Castle castle)
	{
		_castle = castle;
		
		SiegeScheduleDate schedule = SiegeScheduleData.getInstance().getScheduleDateForCastleId(_castle.getResidenceId());
		if (schedule != null && schedule.siegeEnabled())
		{
			startAutoTask();
		}
	}

	public class ScheduleEndSiegeTask: Runnable
	{
		private readonly Siege _siege;
		private Castle _castleInst;

		public ScheduleEndSiegeTask(Siege siege, Castle pCastle)
		{
			_siege = siege;
			_castleInst = pCastle;
		}

		public void run()
		{
			if (!_siege._isInProgress)
			{
				return;
			}

			try
			{
				TimeSpan timeRemaining = _siege._siegeEndDate - DateTime.Now;
				if (timeRemaining > TimeSpan.FromHours(1))
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_CASTLE_SIEGE_ENDS_IN_S1_H);
					sm.Params.addInt(2);
					_siege.announceToPlayer(sm, true);
					ThreadPool.schedule(new ScheduleEndSiegeTask(_siege, _castleInst), timeRemaining.Subtract(TimeSpan.FromHours(1))); // Prepare task for 1 hr left.
				}
				else if (timeRemaining <= TimeSpan.FromHours(1) && timeRemaining > TimeSpan.FromMinutes(10))
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_CASTLE_SIEGE_ENDS_IN_S1_MIN);
					sm.Params.addInt((int)timeRemaining.TotalMinutes);
					_siege.announceToPlayer(sm, true);
					ThreadPool.schedule(new ScheduleEndSiegeTask(_siege, _castleInst), timeRemaining.Subtract(TimeSpan.FromMinutes(10))); // Prepare task for 10 minute left.
				}
				else if (timeRemaining <= TimeSpan.FromMinutes(10) && timeRemaining > TimeSpan.FromMinutes(5))
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_CASTLE_SIEGE_ENDS_IN_S1_MIN);
					sm.Params.addInt((int)timeRemaining.TotalMinutes);
					_siege.announceToPlayer(sm, true);
					ThreadPool.schedule(new ScheduleEndSiegeTask(_siege, _castleInst), timeRemaining.Subtract(TimeSpan.FromMinutes(5))); // Prepare task for 5 minute left.
				}
				else if (timeRemaining <= TimeSpan.FromMinutes(5) && timeRemaining > TimeSpan.FromSeconds(10))
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_CASTLE_SIEGE_ENDS_IN_S1_MIN);
					sm.Params.addInt((int)timeRemaining.TotalMinutes);
					_siege.announceToPlayer(sm, true);
					ThreadPool.schedule(new ScheduleEndSiegeTask(_siege, _castleInst), timeRemaining.Subtract(TimeSpan.FromSeconds(10))); // Prepare task for 10 seconds count down
				}
				else if (timeRemaining <= TimeSpan.FromSeconds(10) && timeRemaining > TimeSpan.Zero)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_CASTLE_SIEGE_ENDS_IN_S1_SEC);
					sm.Params.addInt((int)timeRemaining.TotalSeconds);
					_siege.announceToPlayer(sm, true);
					ThreadPool.schedule(new ScheduleEndSiegeTask(_siege, _castleInst), timeRemaining); // Prepare task for second count down
				}
				else
				{
					_castleInst.getSiege().endSiege();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": ", e);
			}
		}
	}

	public class ScheduleStartSiegeTask: Runnable
	{
		private readonly Siege _siege;
		private Castle _castleInst;

		public ScheduleStartSiegeTask(Siege siege, Castle pCastle)
		{
			_siege = siege;
			_castleInst = pCastle;
		}

		public void run()
		{
			_siege._scheduledStartSiegeTask.cancel(false);
			if (_siege._isInProgress)
			{
				return;
			}

			try
			{
				DateTime currentTime = DateTime.Now;
				if (!_siege._castle.isTimeRegistrationOver())
				{
					TimeSpan regTimeRemaining = _siege.getTimeRegistrationOverDate() - currentTime;
					if (regTimeRemaining > TimeSpan.Zero)
					{
						_siege._scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(_siege, _castleInst), regTimeRemaining);
						return;
					}
					_siege.endTimeRegistration(true);
				}

				TimeSpan timeRemaining = _siege.getSiegeDate() - currentTime;
				if (timeRemaining > TimeSpan.FromMilliseconds(86400000))
				{
					_siege._scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(_siege, _castleInst), timeRemaining - TimeSpan.FromMilliseconds(86400000)); // Prepare task for 24 before siege start to end registration
				}
				else if (timeRemaining <= TimeSpan.FromMilliseconds(86400000) && timeRemaining > TimeSpan.FromMilliseconds(13600000))
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_REGISTRATION_TERM_FOR_S1_HAS_ENDED);
					sm.Params.addCastleId(_siege._castle.getResidenceId());
					Broadcast.toAllOnlinePlayers(sm);
					_siege._isRegistrationOver = true;
					_siege.clearSiegeWaitingClan();
					_siege._scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(_siege, _castleInst), timeRemaining - TimeSpan.FromMilliseconds(13600000)); // Prepare task for 1 hr left before siege start.
				}
				else if (timeRemaining <= TimeSpan.FromMilliseconds(13600000) && timeRemaining > TimeSpan.FromMilliseconds(600000))
				{
					_siege._scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(_siege, _castleInst), timeRemaining - TimeSpan.FromMilliseconds(600000)); // Prepare task for 10 minute left.
				}
				else if (timeRemaining <= TimeSpan.FromMilliseconds(600000) && timeRemaining > TimeSpan.FromMilliseconds(300000))
				{
					_siege._scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(_siege, _castleInst), timeRemaining - TimeSpan.FromMilliseconds(300000)); // Prepare task for 5 minute left.
				}
				else if (timeRemaining <= TimeSpan.FromMilliseconds(300000) && timeRemaining > TimeSpan.FromMilliseconds(10000))
				{
					_siege._scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(_siege, _castleInst), timeRemaining - TimeSpan.FromMilliseconds(10000)); // Prepare task for 10 seconds count down
				}
				else if (timeRemaining <= TimeSpan.FromMilliseconds(10000) && timeRemaining > TimeSpan.FromMilliseconds(0))
				{
					_siege._scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(_siege, _castleInst), timeRemaining); // Prepare task for second count down
				}
				else
				{
					_castleInst.getSiege().startSiege();
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": ", e);
			}
		}
	}

	public void endSiege()
	{
		if (_isInProgress)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_S1_SIEGE_HAS_FINISHED);
			sm.Params.addCastleId(_castle.getResidenceId());
			Broadcast.toAllOnlinePlayers(sm);
			Broadcast.toAllOnlinePlayers(new PlaySoundPacket("systemmsg_eu.18"));
			if (_castle.getOwnerId() > 0)
			{
				Clan clan = ClanTable.getInstance().getClan(getCastle().getOwnerId());
				sm = new SystemMessagePacket(SystemMessageId.CLAN_S1_IS_VICTORIOUS_OVER_S2_S_CASTLE_SIEGE);
				sm.Params.addString(clan.getName());
				sm.Params.addCastleId(_castle.getResidenceId());
				Broadcast.toAllOnlinePlayers(sm);

				if (clan.getId() == _firstOwnerClanId)
				{
					// Owner is unchanged
					clan.increaseBloodAllianceCount();
				}
				else
				{
					_castle.setTicketBuyCount(0);
					foreach (ClanMember member in clan.getMembers())
					{
						if (member != null)
						{
							Player player = member.getPlayer();
							if (player != null && player.isNoble())
							{
								Hero.getInstance().setCastleTaken(player.getObjectId(), getCastle().getResidenceId());
							}
						}
					}
				}
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.THE_SIEGE_OF_S1_HAS_ENDED_IN_A_DRAW);
				sm.Params.addCastleId(_castle.getResidenceId());
				Broadcast.toAllOnlinePlayers(sm);
			}

			foreach (SiegeClan attackerClan in getAttackerClans())
			{
				Clan clan = ClanTable.getInstance().getClan(attackerClan.getClanId());
				if (clan == null)
				{
					continue;
				}

				foreach (Player member in clan.getOnlineMembers(0))
				{
					member.checkItemRestriction();
				}

				clan.clearSiegeKills();
				clan.clearSiegeDeaths();
			}

			foreach (SiegeClan defenderClan in getDefenderClans())
			{
				Clan clan = ClanTable.getInstance().getClan(defenderClan.getClanId());
				if (clan == null)
				{
					continue;
				}

				foreach (Player member in clan.getOnlineMembers(0))
				{
					member.checkItemRestriction();
				}

				clan.clearSiegeKills();
				clan.clearSiegeDeaths();
			}

			_castle.updateClansReputation();
			removeFlags(); // Removes all flags. Note: Remove flag before teleporting players
			teleportPlayer(SiegeTeleportWhoType.NotOwner, TeleportWhereType.TOWN); // Teleport to the second closest town
			_isInProgress = false; // Flag so that siege instance can be started
			updatePlayerSiegeStateFlags(true);
			saveCastleSiege(); // Save castle specific data
			clearSiegeClan(); // Clear siege clan from db
			removeTowers(); // Remove all towers from this castle
			SiegeGuardManager.getInstance().unspawnSiegeGuard(getCastle()); // Remove all spawned siege guard from this castle
			if (_castle.getOwnerId() > 0)
			{
				SiegeGuardManager.getInstance().removeSiegeGuards(getCastle());
			}
			_castle.spawnDoor(); // Respawn door to castle
			_castle.setFirstMidVictory(false);
			_castle.getZone().setActive(false);
			_castle.getZone().updateZoneStatusForCharactersInside();
			_castle.getZone().setSiegeInstance(null);

			// Notify to scripts.
			EventContainer castleEvents = getCastle().Events;
			if (castleEvents.HasSubscribers<OnCastleSiegeFinish>())
			{
				castleEvents.NotifyAsync(new OnCastleSiegeFinish(this));
			}
		}
	}

	private void removeDefender(SiegeClan sc)
	{
		if (sc != null)
		{
			getDefenderClans().Remove(sc);
		}
	}

	private void removeAttacker(SiegeClan sc)
	{
		if (sc != null)
		{
			getAttackerClans().Remove(sc);
		}
	}

	private void addDefender(SiegeClan sc, SiegeClanType type)
	{
		if (sc == null)
		{
			return;
		}
		sc.setType(type);
		getDefenderClans().Add(sc);
	}

	private void addAttacker(SiegeClan sc)
	{
		if (sc == null)
		{
			return;
		}
		sc.setType(SiegeClanType.ATTACKER);
		getAttackerClans().Add(sc);
	}

	/**
	 * When control of castle changed during siege.
	 */
	public void midVictory()
	{
		if (_isInProgress) // Siege still in progress
		{
			if (_castle.getOwnerId() > 0)
			{
				SiegeGuardManager.getInstance().removeSiegeGuards(getCastle()); // Remove all merc entry from db
			}

			if (getDefenderClans().Count == 0 && // If defender doesn't exist (Pc vs Npc)
				getAttackerClans().Count == 1) // Only 1 attacker
			{
				SiegeClan scNewOwner = getAttackerClan(_castle.getOwnerId());
				removeAttacker(scNewOwner);
				addDefender(scNewOwner, SiegeClanType.OWNER);
				endSiege();
				return;
			}

			if (_castle.getOwnerId() > 0)
			{
				// If defender doesn't exist (Pc vs Npc) and only an alliance attacks
				int? allyId = ClanTable.getInstance().getClan(getCastle().getOwnerId()).getAllyId();
				if (getDefenderClans().Count == 0 && allyId != null)
				{
					bool allinsamealliance = true;
					foreach (SiegeClan sc in getAttackerClans())
					{
						if (sc != null && ClanTable.getInstance().getClan(sc.getClanId()).getAllyId() != allyId)
						{
							allinsamealliance = false;
						}
					}
					if (allinsamealliance)
					{
						SiegeClan scNewOwner = getAttackerClan(_castle.getOwnerId());
						removeAttacker(scNewOwner);
						addDefender(scNewOwner, SiegeClanType.OWNER);
						endSiege();
						return;
					}
				}

				foreach (SiegeClan sc in getDefenderClans())
				{
					if (sc != null)
					{
						removeDefender(sc);
						addAttacker(sc);
					}
				}

				SiegeClan scNewOwner1 = getAttackerClan(_castle.getOwnerId());
				removeAttacker(scNewOwner1);
				addDefender(scNewOwner1, SiegeClanType.OWNER);

				// The player's clan is in an alliance
				foreach (Clan clan in ClanTable.getInstance().getClanAllies(allyId ?? 0)) // TODO: ???
				{
					SiegeClan sc = getAttackerClan(clan.getId());
					if (sc != null)
					{
						removeAttacker(sc);
						addDefender(sc, SiegeClanType.DEFENDER);
					}
				}

				_castle.setFirstMidVictory(true);
				teleportPlayer(SiegeTeleportWhoType.Attacker, TeleportWhereType.SIEGEFLAG); // Teleport to the second closest town
				teleportPlayer(SiegeTeleportWhoType.Spectator, TeleportWhereType.TOWN); // Teleport to the second closest town
				removeDefenderFlags(); // Removes defenders' flags
				_castle.removeUpgrade(); // Remove all castle upgrade
				_castle.spawnDoor(true); // Respawn door to castle but make them weaker (50% hp)
				removeTowers(); // Remove all towers from this castle
				_controlTowerCount = 0; // Each new siege midvictory CT are completely respawned.
				spawnControlTower();
				spawnFlameTower();
				updatePlayerSiegeStateFlags(false);

				// Notify to scripts.
				EventContainer castleEvents = getCastle().Events;
				if (castleEvents.HasSubscribers<OnCastleSiegeOwnerChange>())
				{
					castleEvents.NotifyAsync(new OnCastleSiegeOwnerChange(this));
				}
			}
		}
	}

	/**
	 * When siege starts.
	 */
	public void startSiege()
	{
		if (!_isInProgress)
		{
			SystemMessagePacket sm;
			_firstOwnerClanId = _castle.getOwnerId();
			if (getAttackerClans().Count == 0)
			{
				if (_firstOwnerClanId <= 0)
				{
					sm = new SystemMessagePacket(SystemMessageId.THE_SIEGE_OF_S1_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_INTEREST);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S1_S_SIEGE_WAS_CANCELED_BECAUSE_THERE_WERE_NO_CLANS_THAT_PARTICIPATED);
					Clan ownerClan = ClanTable.getInstance().getClan(_firstOwnerClanId);
					ownerClan.increaseBloodAllianceCount();
				}

				sm.Params.addCastleId(_castle.getResidenceId());
				Broadcast.toAllOnlinePlayers(sm);
				saveCastleSiege();
				return;
			}

			_isNormalSide = true; // Atk is now atk
			_isInProgress = true; // Flag so that same siege instance cannot be started again
			loadSiegeClan(); // Load siege clan from db
			updatePlayerSiegeStateFlags(false);
			updatePlayerSiegeStateFlags(false); // This fixes icons between allies because it first shows as an enemy for unknown reasons
			teleportPlayer(SiegeTeleportWhoType.NotOwner, TeleportWhereType.TOWN); // Teleport to the closest town
			_controlTowerCount = 0;
			spawnControlTower(); // Spawn control tower
			spawnFlameTower(); // Spawn control tower
			_castle.spawnDoor(); // Spawn door
			spawnSiegeGuard(); // Spawn siege guard
			SiegeGuardManager.getInstance().deleteTickets(getCastle().getResidenceId()); // remove the tickets from the ground
			_castle.getZone().setSiegeInstance(this);
			_castle.getZone().setActive(true);
			_castle.getZone().updateZoneStatusForCharactersInside();

			// Schedule a task to prepare auto siege end
			_siegeEndDate = DateTime.UtcNow + SiegeManager.getInstance().getSiegeLength();
			ThreadPool.schedule(new ScheduleEndSiegeTask(this, _castle), 1000); // Prepare auto end task

			sm = new SystemMessagePacket(SystemMessageId.S1_THE_SIEGE_HAS_BEGUN);
			sm.Params.addCastleId(_castle.getResidenceId());
			Broadcast.toAllOnlinePlayers(sm);
			Broadcast.toAllOnlinePlayers(new PlaySoundPacket("systemmsg_eu.17"));
			foreach (Player player in World.getInstance().getPlayers())
			{
				SiegeManager.getInstance().sendSiegeInfo(player);
			}

			// Notify to scripts.
			EventContainer castleEvents = getCastle().Events;
			if (castleEvents.HasSubscribers<OnCastleSiegeStart>())
			{
				castleEvents.NotifyAsync(new OnCastleSiegeStart(this));
			}
		}
	}

	/**
	 * Announce to player.
	 * @param message The SystemMessagePacket to send to player
	 * @param bothSides True - broadcast to both attackers and defenders. False - only to defenders.
	 */
	public void announceToPlayer(SystemMessagePacket message, bool bothSides)
	{
		foreach (SiegeClan siegeClans in getDefenderClans())
		{
			Clan clan = ClanTable.getInstance().getClan(siegeClans.getClanId());
			if (clan != null)
			{
				foreach (Player member in clan.getOnlineMembers(0))
				{
					member.sendPacket(message);
				}
			}
		}

		if (bothSides)
		{
			foreach (SiegeClan siegeClans in getAttackerClans())
			{
				Clan clan = ClanTable.getInstance().getClan(siegeClans.getClanId());
				if (clan != null)
				{
					foreach (Player member in clan.getOnlineMembers(0))
					{
						member.sendPacket(message);
					}
				}
			}
		}
	}

	public void updatePlayerSiegeStateFlags(bool clear)
	{
		Clan clan;
		foreach (SiegeClan siegeclan in getAttackerClans())
		{
			if (siegeclan == null)
			{
				continue;
			}

			clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			foreach (Player member in clan.getOnlineMembers(0))
			{
				if (clear)
				{
					member.setSiegeState((byte) 0);
					member.setSiegeSide(0);
					member.setInSiege(false);
					member.stopFameTask();
				}
				else
				{
					member.setSiegeState((byte) 1);
					member.setSiegeSide(_castle.getResidenceId());
					if (checkIfInZone(member))
					{
						member.setInSiege(true);
						member.startFameTask(TimeSpan.FromMilliseconds(Config.CASTLE_ZONE_FAME_TASK_FREQUENCY * 1000), Config.CASTLE_ZONE_FAME_AQUIRE_POINTS);
					}
				}
				member.updateUserInfo();
				World.getInstance().forEachVisibleObject<Player>(member, player =>
				{
					if (!member.isVisibleFor(player))
					{
						return;
					}

					long relation = member.getRelation(player);
					bool isAutoAttackable = member.isAutoAttackable(player);
					RelationCache oldrelation = member.getKnownRelations().get(player.getObjectId());
					if (oldrelation == null || oldrelation.getRelation() != relation || oldrelation.isAutoAttackable() != isAutoAttackable)
					{
						RelationChangedPacket rc = new RelationChangedPacket();
						rc.addRelation(member, relation, isAutoAttackable);
						if (member.hasSummon())
						{
							Summon pet = member.getPet();
							if (pet != null)
							{
								rc.addRelation(pet, relation, isAutoAttackable);
							}
							if (member.hasServitors())
							{
								member.getServitors().Values.ForEach(s => rc.addRelation(s, relation, isAutoAttackable));
							}
						}
						player.sendPacket(rc);
						member.getKnownRelations().put(player.getObjectId(), new RelationCache(relation, isAutoAttackable));
					}
				});
			}
		}
		foreach (SiegeClan siegeclan in getDefenderClans())
		{
			if (siegeclan == null)
			{
				continue;
			}

			clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			foreach (Player member in clan.getOnlineMembers(0))
			{
				if (clear)
				{
					member.setSiegeState((byte) 0);
					member.setSiegeSide(0);
					member.setInSiege(false);
					member.stopFameTask();
				}
				else
				{
					member.setSiegeState((byte) 2);
					member.setSiegeSide(_castle.getResidenceId());
					if (checkIfInZone(member))
					{
						member.setInSiege(true);
						member.startFameTask(TimeSpan.FromMilliseconds(Config.CASTLE_ZONE_FAME_TASK_FREQUENCY * 1000), Config.CASTLE_ZONE_FAME_AQUIRE_POINTS);
					}
				}
				member.updateUserInfo();
				World.getInstance().forEachVisibleObject<Player>(member, player =>
				{
					if (!member.isVisibleFor(player))
					{
						return;
					}

					long relation = member.getRelation(player);
					bool isAutoAttackable = member.isAutoAttackable(player);
					RelationCache oldrelation = member.getKnownRelations().get(player.getObjectId());
					if (oldrelation == null || oldrelation.getRelation() != relation || oldrelation.isAutoAttackable() != isAutoAttackable)
					{
						RelationChangedPacket rc = new RelationChangedPacket();
						rc.addRelation(member, relation, isAutoAttackable);
						if (member.hasSummon())
						{
							Summon pet = member.getPet();
							if (pet != null)
							{
								rc.addRelation(pet, relation, isAutoAttackable);
							}
							if (member.hasServitors())
							{
								member.getServitors().Values.ForEach(s => rc.addRelation(s, relation, isAutoAttackable));
							}
						}
						player.sendPacket(rc);
						member.getKnownRelations().put(player.getObjectId(), new RelationCache(relation, isAutoAttackable));
					}
				});
			}
		}
	}

	/**
	 * Approve clan as defender for siege
	 * @param clanId The int of player's clan id
	 */
	public void approveSiegeDefenderClan(int clanId)
	{
		if (clanId <= 0)
		{
			return;
		}
		saveSiegeClan(ClanTable.getInstance().getClan(clanId), DEFENDER, true);
		loadSiegeClan();
	}

	/**
	 * @param object
	 * @return true if object is inside the zone
	 */
	public bool checkIfInZone(WorldObject obj)
	{
		return checkIfInZone(obj.Location.Location3D);
	}

	/**
	 * @param x
	 * @param y
	 * @param z
	 * @return true if object is inside the zone
	 */
	public bool checkIfInZone(Location3D location)
	{
		return _isInProgress && _castle.checkIfInZone(location); // Castle zone during siege
	}

	/**
	 * Return true if clan is attacker
	 * @param clan The Clan of the player
	 */
	public bool checkIsAttacker(Clan clan)
	{
		return getAttackerClan(clan) != null;
	}

	/**
	 * Return true if clan is defender
	 * @param clan The Clan of the player
	 */
	public bool checkIsDefender(Clan clan)
	{
		return getDefenderClan(clan) != null;
	}

	/**
	 * @param clan The Clan of the player
	 * @return true if clan is defender waiting approval
	 */
	public bool checkIsDefenderWaiting(Clan clan)
	{
		return getDefenderWaitingClan(clan) != null;
	}

	/** Clear all registered siege clans from database for castle */
	public void clearSiegeClan()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = _castle.getResidenceId();
			ctx.CastleSiegeClans.Where(r => r.CastleId == castleId).ExecuteDelete();

			if (_castle.getOwnerId() > 0)
			{
				int clanId = _castle.getOwnerId();
				ctx.CastleSiegeClans.Where(r => r.ClanId == clanId).ExecuteDelete();
			}

			getAttackerClans().Clear();
			getDefenderClans().Clear();
			_defenderWaitingClans.clear();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Exception: clearSiegeClan(): " + e);
		}
	}

	/** Clear all siege clans waiting for approval from database for castle */
	public void clearSiegeWaitingClan()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = _castle.getResidenceId();
			ctx.CastleSiegeClans.Where(r => r.CastleId == castleId && r.Type == 2).ExecuteDelete();

			_defenderWaitingClans.clear();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Exception: clearSiegeWaitingClan(): " + e);
		}
	}

	/** Return list of Player registered as attacker in the zone. */
	public List<Player> getAttackersInZone()
	{
		List<Player> result = new();
		foreach (SiegeClan siegeclan in getAttackerClans())
		{
			Clan clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
			if (clan != null)
			{
				foreach (Player member in clan.getOnlineMembers(0))
				{
					if (member.isInSiege())
					{
						result.Add(member);
					}
				}
			}
		}
		return result;
	}

	/**
	 * @return list of Player in the zone.
	 */
	public List<Player> getPlayersInZone()
	{
		return _castle.getZone().getPlayersInside();
	}

	/**
	 * @return list of Player owning the castle in the zone.
	 */
	public List<Player> getOwnersInZone()
	{
		List<Player> result = new();
		foreach (SiegeClan siegeclan in getDefenderClans())
		{
			if (siegeclan.getClanId() == _castle.getOwnerId())
			{
				Clan clan = ClanTable.getInstance().getClan(siegeclan.getClanId());
				if (clan != null)
				{
					foreach (Player member in clan.getOnlineMembers(0))
					{
						if (member.isInSiege())
						{
							result.Add(member);
						}
					}
				}
			}
		}
		return result;
	}

	/**
	 * @return list of Player not registered as attacker or defender in the zone.
	 */
	public List<Player> getSpectatorsInZone()
	{
		List<Player> result = new();
		foreach (Player player in _castle.getZone().getPlayersInside())
		{
			if (!player.isInSiege())
			{
				result.Add(player);
			}
		}
		return result;
	}

	/**
	 * Control Tower was killed
	 * @param ct
	 */
	public void killedCT(Npc ct)
	{
		_controlTowerCount = Math.Max(_controlTowerCount - 1, 0);
	}

	/**
	 * Remove the flag that was killed
	 * @param flag
	 */
	public void killedFlag(Npc flag)
	{
		getAttackerClans().ForEach(siegeClan => siegeClan.removeFlag(flag));
	}

	/**
	 * Display list of registered clans
	 * @param player
	 */
	public void listRegisterClan(Player player)
	{
		player.sendPacket(new SiegeInfoPacket(_castle, player));
	}

	/**
	 * Register clan as attacker
	 * @param player The Player of the player trying to register
	 */
	public void registerAttacker(Player player)
	{
		registerAttacker(player, false);
	}

	public void registerAttacker(Player player, bool force)
	{
		if (player.getClan() == null)
		{
			return;
		}
		int? allyId = null;
		if (_castle.getOwnerId() != 0)
		{
			allyId = ClanTable.getInstance().getClan(getCastle().getOwnerId()).getAllyId();
		}
		if (allyId != 0 && player.getClan().getAllyId() == allyId && !force)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_REGISTER_AS_AN_ATTACKER_BECAUSE_YOU_ARE_IN_AN_ALLIANCE_WITH_THE_CASTLE_OWNING_CLAN);
			return;
		}

		if (force)
		{
			if (SiegeManager.getInstance().checkIsRegistered(player.getClan(), getCastle().getResidenceId()))
			{
				player.sendPacket(SystemMessageId.YOU_HAVE_ALREADY_REQUESTED_A_CASTLE_SIEGE);
			}
			else
			{
				saveSiegeClan(player.getClan(), ATTACKER, false); // Save to database
			}
			return;
		}

		if (checkIfCanRegister(player, ATTACKER))
		{
			saveSiegeClan(player.getClan(), ATTACKER, false); // Save to database
		}
	}

	/**
	 * Register a clan as defender.
	 * @param player the player to register
	 */
	public void registerDefender(Player player)
	{
		registerDefender(player, false);
	}

	public void registerDefender(Player player, bool force)
	{
		if (_castle.getOwnerId() <= 0)
		{
			player.sendMessage("You cannot register as a defender because " + _castle.getName() + " is owned by NPC.");
			return;
		}

		if (force)
		{
			if (SiegeManager.getInstance().checkIsRegistered(player.getClan(), getCastle().getResidenceId()))
			{
				player.sendPacket(SystemMessageId.YOU_HAVE_ALREADY_REQUESTED_A_CASTLE_SIEGE);
			}
			else
			{
				saveSiegeClan(player.getClan(), DEFENDER_NOT_APPROVED, false); // Save to database
			}
			return;
		}

		if (checkIfCanRegister(player, DEFENDER_NOT_APPROVED))
		{
			saveSiegeClan(player.getClan(), DEFENDER_NOT_APPROVED, false); // Save to database
		}
	}

	/**
	 * Remove clan from siege
	 * @param clanId The int of player's clan id
	 */
	public void removeSiegeClan(int clanId)
	{
		if (clanId <= 0)
		{
			return;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = _castle.getResidenceId();
			ctx.CastleSiegeClans.Where(r => r.CastleId == castleId && r.ClanId == clanId).ExecuteDelete();

			loadSiegeClan();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Exception: removeSiegeClan(): " + e);
		}
	}

	/**
	 * Remove clan from siege
	 * @param clan clan being removed
	 */
	public void removeSiegeClan(Clan clan)
	{
		if (clan == null || clan.getCastleId() == getCastle().getResidenceId() || !SiegeManager.getInstance().checkIsRegistered(clan, getCastle().getResidenceId()))
		{
			return;
		}
		removeSiegeClan(clan.getId());
	}

	/**
	 * Remove clan from siege
	 * @param player The Player of player/clan being removed
	 */
	public void removeSiegeClan(Player player)
	{
		removeSiegeClan(player.getClan());
	}

	/**
	 * Start the auto tasks.
	 */
	public void startAutoTask()
	{
		correctSiegeDateTime();

		LOGGER.Info("Siege of " + _castle.getName() + ": " + _castle.getSiegeDate());
		loadSiegeClan();

		// Schedule siege auto start
		if (_scheduledStartSiegeTask != null)
		{
			_scheduledStartSiegeTask.cancel(false);
		}
		_scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, _castle), 1000);
		startInfoTask();
	}

	private void startInfoTask()
	{
		if (_scheduledSiegeInfoTask != null)
		{
			_scheduledSiegeInfoTask.cancel(false);
		}

		_scheduledSiegeInfoTask = ThreadPool.schedule(() =>
		{
			foreach (Player player in World.getInstance().getPlayers())
			{
				SiegeManager.getInstance().sendSiegeInfo(player, _castle.getResidenceId());
			}
		}, Algorithms.Max(TimeSpan.Zero, getSiegeDate() - DateTime.UtcNow - TimeSpan.FromMilliseconds(3600000)));
	}

	/**
	 * Teleport players
	 * @param teleportWho
	 * @param teleportWhere
	 */
	public void teleportPlayer(SiegeTeleportWhoType teleportWho, TeleportWhereType teleportWhere)
	{
		List<Player> players;
		switch (teleportWho)
		{
			case SiegeTeleportWhoType.Owner:
			{
				players = getOwnersInZone();
				break;
			}
			case SiegeTeleportWhoType.NotOwner:
			{
				players = _castle.getZone().getPlayersInside();
				players.RemoveAll(player =>
					player == null || player.inObserverMode() ||
					(player.getClanId() > 0 && player.getClanId() == _castle.getOwnerId()));

				break;
			}
			case SiegeTeleportWhoType.Attacker:
			{
				players = getAttackersInZone();
				break;
			}
			case SiegeTeleportWhoType.Spectator:
			{
				players = getSpectatorsInZone();
				break;
			}
			default:
			{
				players = new();
				break;
			}
		}

		foreach (Player player in players)
		{
			if (player.canOverrideCond(PlayerCondOverride.CASTLE_CONDITIONS) || player.isJailed())
			{
				continue;
			}

			player.teleToLocation(teleportWhere);
		}
	}

	/**
	 * Add clan as attacker
	 * @param clanId The int of clan's id
	 */
	private void addAttacker(int clanId)
	{
		getAttackerClans().Add(new SiegeClan(clanId, SiegeClanType.ATTACKER)); // Add registered attacker to attacker list
	}

	/**
	 * Add clan as defender
	 * @param clanId The int of clan's id
	 */
	private void addDefender(int clanId)
	{
		getDefenderClans().Add(new SiegeClan(clanId, SiegeClanType.DEFENDER)); // Add registered defender to defender list
	}

	/**
	 * <p>
	 * Add clan as defender with the specified type
	 * </p>
	 * @param clanId The int of clan's id
	 * @param type the type of the clan
	 */
	private void addDefender(int clanId, SiegeClanType type)
	{
		getDefenderClans().Add(new SiegeClan(clanId, type));
	}

	/**
	 * Add clan as defender waiting approval
	 * @param clanId The int of clan's id
	 */
	private void addDefenderWaiting(int clanId)
	{
		_defenderWaitingClans.add(new SiegeClan(clanId, SiegeClanType.DEFENDER_PENDING)); // Add registered defender to defender list
	}

	/**
	 * @param player The Player of the player trying to register
	 * @param typeId -1 = owner 0 = defender, 1 = attacker, 2 = defender waiting
	 * @return true if the player can register.
	 */
	private bool checkIfCanRegister(Player player, byte typeId)
	{
		if (_isRegistrationOver)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_DEADLINE_TO_REGISTER_FOR_THE_SIEGE_OF_S1_HAS_PASSED);
			sm.Params.addCastleId(_castle.getResidenceId());
			player.sendPacket(sm);
		}
		else if (_isInProgress)
		{
			player.sendPacket(SystemMessageId.THIS_IS_NOT_THE_TIME_FOR_SIEGE_REGISTRATION_AND_SO_REGISTRATION_AND_CANCELLATION_CANNOT_BE_DONE);
		}
		else if (player.getClan() == null || player.getClan().getLevel() < SiegeManager.getInstance().getSiegeClanMinLevel())
		{
			player.sendPacket(SystemMessageId.ONLY_CLANS_OF_LEVEL_3_OR_ABOVE_MAY_REGISTER_FOR_A_CASTLE_SIEGE);
		}
		else if (player.getClan().getId() == _castle.getOwnerId())
		{
			player.sendPacket(SystemMessageId.CASTLE_OWNING_CLANS_ARE_AUTOMATICALLY_REGISTERED_ON_THE_DEFENDING_SIDE);
		}
		else if (player.getClan().getCastleId() > 0)
		{
			player.sendPacket(SystemMessageId.A_CLAN_THAT_OWNS_A_CASTLE_CANNOT_PARTICIPATE_IN_ANOTHER_SIEGE);
		}
		else if (SiegeManager.getInstance().checkIsRegistered(player.getClan(), getCastle().getResidenceId()))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_ALREADY_REQUESTED_A_CASTLE_SIEGE);
		}
		else if (checkIfAlreadyRegisteredForSameDay(player.getClan()))
		{
			player.sendPacket(SystemMessageId.YOUR_APPLICATION_HAS_BEEN_DENIED_BECAUSE_YOU_HAVE_ALREADY_SUBMITTED_A_REQUEST_FOR_ANOTHER_CASTLE_SIEGE);
		}
		else if (typeId == ATTACKER && getAttackerClans().Count >= SiegeManager.getInstance().getAttackerMaxClans())
		{
			player.sendPacket(SystemMessageId.NO_MORE_REGISTRATIONS_MAY_BE_ACCEPTED_FOR_THE_ATTACKER_SIDE);
		}
		else if ((typeId == DEFENDER || typeId == DEFENDER_NOT_APPROVED || typeId == OWNER) && getDefenderClans().Count + getDefenderWaitingClans().Count >= SiegeManager.getInstance().getDefenderMaxClans())
		{
			player.sendPacket(SystemMessageId.NO_MORE_REGISTRATIONS_MAY_BE_ACCEPTED_FOR_THE_DEFENDER_SIDE);
		}
		// In Classic, only lvl 3-4 clans are able to participate in the Gludio Castle Siege.
		else if (_castle.getResidenceId() == 1 && player.getClan().getLevel() >= 5)
		{
			player.sendPacket(SystemMessageId.ONLY_LEVEL_3_4_CLANS_CAN_PARTICIPATE_IN_CASTLE_SIEGE);
		}
		else
		{
			return true;
		}
		return false;
	}

	/**
	 * @param clan The Clan of the player trying to register
	 * @return true if the clan has already registered to a siege for the same day.
	 */
	public bool checkIfAlreadyRegisteredForSameDay(Clan clan)
	{
		foreach (Siege siege in SiegeManager.getInstance().getSieges())
		{
			if (siege == this)
			{
				continue;
			}
			if (siege.getSiegeDate().DayOfWeek == getSiegeDate().DayOfWeek)
			{
				if (siege.checkIsAttacker(clan))
				{
					return true;
				}
				if (siege.checkIsDefender(clan))
				{
					return true;
				}
				if (siege.checkIsDefenderWaiting(clan))
				{
					return true;
				}
			}
		}
		return false;
	}

	/**
	 * Return the correct siege date as Calendar.
	 */
	public void correctSiegeDateTime()
	{
		bool corrected = false;
		if (getCastle().getSiegeDate() < DateTime.UtcNow)
		{
			// Since siege has past reschedule it to the next one
			// This is usually caused by server being down
			corrected = true;
			setNextSiegeDate();
		}

		if (corrected)
		{
			saveSiegeDate();
		}
	}

	/** Load siege clans. */
	private void loadSiegeClan()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = _castle.getResidenceId();

     		getAttackerClans().Clear();
			getDefenderClans().Clear();
			_defenderWaitingClans.clear();

			// Add castle owner as defender (add owner first so that they are on the top of the defender list)
			if (_castle.getOwnerId() > 0)
			{
				addDefender(_castle.getOwnerId(), SiegeClanType.OWNER);
			}

			var query = ctx.CastleSiegeClans.Where(r => r.CastleId == castleId);
			foreach (var record in query)
			{
				int typeId = record.Type;
				if (typeId == DEFENDER)
				{
					addDefender(record.ClanId);
				}
				else if (typeId == ATTACKER)
				{
					addAttacker(record.ClanId);
				}
				else if (typeId == DEFENDER_NOT_APPROVED)
				{
					addDefenderWaiting(record.ClanId);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Exception: loadSiegeClan(): " + e);
		}
	}

	/** Remove all spawned towers. */
	private void removeTowers()
	{
		foreach (FlameTower ct in _flameTowers)
		{
			ct.deleteMe();
		}

		foreach (ControlTower ct in _controlTowers)
		{
			ct.deleteMe();
		}

		_flameTowers.Clear();
		_controlTowers.Clear();
	}

	/** Remove all flags. */
	private void removeFlags()
	{
		foreach (SiegeClan sc in getAttackerClans())
		{
			if (sc != null)
			{
				sc.removeFlags();
			}
		}
		foreach (SiegeClan sc in getDefenderClans())
		{
			if (sc != null)
			{
				sc.removeFlags();
			}
		}
	}

	/** Remove flags from defenders. */
	private void removeDefenderFlags()
	{
		foreach (SiegeClan sc in getDefenderClans())
		{
			if (sc != null)
			{
				sc.removeFlags();
			}
		}
	}

	/** Save castle siege related to database. */
	private void saveCastleSiege()
	{
		setNextSiegeDate(); // Set the next set date for 2 weeks from now
		// Schedule Time registration end
		_castle.setTimeRegistrationOverDate(DateTime.UtcNow.AddDays(1));
		_castle.setTimeRegistrationOver(false);

		saveSiegeDate(); // Save the new date
		startAutoTask(); // Prepare auto start siege and end registration
	}

	/** Save siege date to database. */
	public void saveSiegeDate()
	{
		if (_scheduledStartSiegeTask != null)
		{
			_scheduledStartSiegeTask.cancel(true);
			_scheduledStartSiegeTask = ThreadPool.schedule(new ScheduleStartSiegeTask(this, _castle), 1000);
		}
		startInfoTask();

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int castleId = _castle.getResidenceId();
			var record = ctx.Castles.SingleOrDefault(r => r.Id == castleId);
			if (record is null)
			{
				record = new DbCastle();
				record.Id = castleId;
				ctx.Castles.Add(record);
			}

			record.SiegeTime = _castle.getSiegeDate();
			record.RegistrationEndTime = _castle.getTimeRegistrationOverDate();
			record.RegistrationTimeOver = _castle.isTimeRegistrationOver();
			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Exception: saveSiegeDate(): " + e);
		}
	}

	/**
	 * Save registration to database.
	 * @param clan The Clan of player
	 * @param typeId -1 = owner 0 = defender, 1 = attacker, 2 = defender waiting
	 * @param isUpdateRegistration
	 */
	private void saveSiegeClan(Clan clan, byte typeId, bool isUpdateRegistration)
	{
		if (clan.getCastleId() > 0)
		{
			return;
		}

		try
		{
			if (typeId == DEFENDER || typeId == DEFENDER_NOT_APPROVED || typeId == OWNER)
			{
				if (getDefenderClans().Count + getDefenderWaitingClans().Count >= SiegeManager.getInstance().getDefenderMaxClans())
				{
					return;
				}
			}
			else if (getAttackerClans().Count >= SiegeManager.getInstance().getAttackerMaxClans())
			{
				return;
			}

			if (!isUpdateRegistration)
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.CastleSiegeClans.Add(new DbCastleSiegeClan()
				{
					ClanId = clan.getId(),
					CastleId = (byte)_castle.getResidenceId()
				});

				ctx.SaveChanges();
			}
			else
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				int castleId = _castle.getResidenceId();
				int clanId = clan.getId();

				ctx.CastleSiegeClans.Where(r => r.CastleId == castleId && r.ClanId == clanId)
					.ExecuteUpdate(s => s.SetProperty(r => r.Type, typeId));
			}

			if (typeId == DEFENDER || typeId == OWNER)
			{
				addDefender(clan.getId());
			}
			else if (typeId == ATTACKER)
			{
				addAttacker(clan.getId());
			}
			else if (typeId == DEFENDER_NOT_APPROVED)
			{
				addDefenderWaiting(clan.getId());
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Exception: saveSiegeClan(Pledge clan, int typeId, bool isUpdateRegistration): " + e);
		}
	}

	/** Set the date for the next siege. */
	private void setNextSiegeDate()
	{
		SiegeScheduleDate holder = SiegeScheduleData.getInstance().getScheduleDateForCastleId(_castle.getResidenceId());
		if (holder == null || !holder.siegeEnabled())
		{
			return;
		}

		DateTime calendar = _castle.getSiegeDate();
		if (calendar < DateTime.UtcNow)
		{
			calendar = DateTime.UtcNow;
		}

		calendar = new DateTime(calendar.Year, calendar.Month, calendar.Day, holder.getHour(), 0, 0);
		while (calendar.DayOfWeek != holder.getDay())
			calendar = calendar.AddDays(1);

		if (CastleManager.getInstance().getSiegeDates(calendar) < holder.getMaxConcurrent())
		{
			CastleManager.getInstance().registerSiegeDate(getCastle().getResidenceId(), calendar);

			SystemMessagePacket sm =
				new SystemMessagePacket(SystemMessageId.S1_HAS_ANNOUNCED_THE_NEXT_CASTLE_SIEGE_TIME);
			sm.Params.addCastleId(_castle.getResidenceId());
			Broadcast.toAllOnlinePlayers(sm);

			// Allow registration for next siege
			_isRegistrationOver = false;
		}
		else
		{
			// Deny registration for next siege
			_isRegistrationOver = true;
		}
	}

	/**
	 * Spawn control tower.
	 */
	private void spawnControlTower()
	{
		try
		{
			foreach (TowerSpawn ts in SiegeManager.getInstance().getControlTowers(getCastle().getResidenceId()))
			{
				Spawn spawn = new Spawn(ts.getId());
				spawn.Location = new Location(ts.getLocation(), 0);
				_controlTowers.Add((ControlTower) spawn.doSpawn(false));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Cannot spawn control tower! " + e);
		}
		_controlTowerCount = _controlTowers.Count;
	}

	/**
	 * Spawn flame tower.
	 */
	private void spawnFlameTower()
	{
		try
		{
			foreach (TowerSpawn ts in SiegeManager.getInstance().getFlameTowers(getCastle().getResidenceId()))
			{
				Spawn spawn = new Spawn(ts.getId());
				spawn.Location = new Location(ts.getLocation(), 0);
				FlameTower tower = (FlameTower)spawn.doSpawn(false);
				tower.setUpgradeLevel(ts.getUpgradeLevel());
				tower.setZoneList(ts.getZoneList());
				_flameTowers.Add(tower);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Cannot spawn flame tower! " + e);
		}
	}

	/**
	 * Spawn siege guard.
	 */
	private void spawnSiegeGuard()
	{
		SiegeGuardManager.getInstance().spawnSiegeGuard(getCastle());

		// Register guard to the closest Control Tower
		// When CT dies, so do all the guards that it controls
		Set<Spawn> spawned = SiegeGuardManager.getInstance().getSpawnedGuards(getCastle().getResidenceId());
		if (!spawned.isEmpty())
		{
			ControlTower closestCt;
			double distance;
			double distanceClosest = 0;
			foreach (Spawn spawn in spawned)
			{
				if (spawn == null)
				{
					continue;
				}

				closestCt = null;
				distanceClosest = int.MaxValue;
				foreach (ControlTower ct in _controlTowers)
				{
					if (ct == null)
					{
						continue;
					}

					distance = ct.DistanceSquare3D(spawn.Location.Location3D);
					if (distance < distanceClosest)
					{
						closestCt = ct;
						distanceClosest = distance;
					}
				}
				if (closestCt != null)
				{
					closestCt.registerGuard(spawn);
				}
			}
		}
	}

	public SiegeClan getAttackerClan(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		return getAttackerClan(clan.getId());
	}

	public SiegeClan getAttackerClan(int clanId)
	{
		foreach (SiegeClan sc in getAttackerClans())
		{
			if (sc != null && sc.getClanId() == clanId)
			{
				return sc;
			}
		}
		return null;
	}

	public ICollection<SiegeClan> getAttackerClans()
	{
		if (_isNormalSide)
		{
			return _attackerClans;
		}
		return _defenderClans;
	}

	public int getAttackerRespawnDelay()
	{
		return SiegeManager.getInstance().getAttackerRespawnDelay();
	}

	public Castle getCastle()
	{
		return _castle;
	}

	public SiegeClan getDefenderClan(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		return getDefenderClan(clan.getId());
	}

	public SiegeClan getDefenderClan(int clanId)
	{
		foreach (SiegeClan sc in getDefenderClans())
		{
			if (sc != null && sc.getClanId() == clanId)
			{
				return sc;
			}
		}
		return null;
	}

	public ICollection<SiegeClan> getDefenderClans()
	{
		if (_isNormalSide)
		{
			return _defenderClans;
		}
		return _attackerClans;
	}

	public SiegeClan getDefenderWaitingClan(Clan clan)
	{
		if (clan == null)
		{
			return null;
		}
		return getDefenderWaitingClan(clan.getId());
	}

	public SiegeClan getDefenderWaitingClan(int clanId)
	{
		foreach (SiegeClan sc in _defenderWaitingClans)
		{
			if (sc != null && sc.getClanId() == clanId)
			{
				return sc;
			}
		}
		return null;
	}
	
	public ICollection<SiegeClan> getDefenderWaitingClans()
	{
		return _defenderWaitingClans;
	}
	
	public bool isInProgress()
	{
		return _isInProgress;
	}
	
	public bool isRegistrationOver()
	{
		return _isRegistrationOver;
	}
	
	public bool isTimeRegistrationOver()
	{
		return _castle.isTimeRegistrationOver();
	}
	
	public DateTime getSiegeDate()
	{
		return _castle.getSiegeDate();
	}
	
	public DateTime getTimeRegistrationOverDate()
	{
		return _castle.getTimeRegistrationOverDate();
	}
	
	public void endTimeRegistration(bool automatic)
	{
		_castle.setTimeRegistrationOver(true);
		if (!automatic)
		{
			saveSiegeDate();
		}
	}
	
	public Set<Npc> getFlag(Clan clan)
	{
		if (clan != null)
		{
			SiegeClan sc = getAttackerClan(clan);
			if (sc != null)
			{
				return sc.getFlag();
			}
		}
		return null;
	}
	
	public int getControlTowerCount()
	{
		return _controlTowerCount;
	}
	
	public bool giveFame()
	{
		return true;
	}
	
	public int getFameFrequency()
	{
		return Config.CASTLE_ZONE_FAME_TASK_FREQUENCY;
	}
	
	public int getFameAmount()
	{
		return Config.CASTLE_ZONE_FAME_AQUIRE_POINTS;
	}
	
	public void updateSiege()
	{
	}
}