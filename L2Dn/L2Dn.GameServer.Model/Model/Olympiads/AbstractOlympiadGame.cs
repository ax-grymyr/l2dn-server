using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author godson, GodKratos, Pere, DS
 */
public abstract class AbstractOlympiadGame
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(AbstractOlympiadGame));
	protected static readonly Logger LOGGER_OLYMPIAD = LogManager.GetLogger("olympiad");

	protected const string POINTS = "olympiad_points";
	protected const string COMP_DONE = "competitions_done";
	protected const string COMP_WON = "competitions_won";
	protected const string COMP_LOST = "competitions_lost";
	protected const string COMP_DRAWN = "competitions_drawn";
	protected const string COMP_DONE_WEEK = "competitions_done_week";
	protected const string COMP_DONE_WEEK_CLASSED = "competitions_done_week_classed";
	protected const string COMP_DONE_WEEK_NON_CLASSED = "competitions_done_week_non_classed";
	protected const string COMP_DONE_WEEK_TEAM = "competitions_done_week_team";

	protected DateTime _startTime;
	protected bool _aborted = false;
	protected readonly int _stadiumId;

	protected AbstractOlympiadGame(int id)
	{
		_stadiumId = id;
	}

	public bool isAborted()
	{
		return _aborted;
	}

	public int getStadiumId()
	{
		return _stadiumId;
	}

	public virtual bool makeCompetitionStart()
	{
		_startTime = DateTime.UtcNow;
		return !_aborted;
	}

	protected void addPointsToParticipant(Participant par, int points)
	{
		par.getStats().OlympiadPoints += points;
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_EARNED_OLYMPIAD_POINTS_X_S2);
		sm.Params.addString(par.getName());
		sm.Params.addInt(points);
		broadcastPacket(sm);
	}

	protected void removePointsFromParticipant(Participant par, int points)
	{
		par.getStats().OlympiadPoints -= points;
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_LOST_OLYMPIAD_POINTS_X_S2);
		sm.Params.addString(par.getName());
		sm.Params.addInt(points);
		broadcastPacket(sm);
	}

	/**
	 * Function return null if player passed all checks or SystemMessage with reason for broadcast to opponent(s).
	 * @param player
	 * @return
	 */
	protected static SystemMessagePacket? checkDefaulted(Player player)
	{
		if (player == null || !player.isOnline())
		{
			return new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_MADE_HASTE_WITH_THEIR_TAIL_BETWEEN_THEIR_LEGS_THE_MATCH_HAS_BEEN_CANCELLED);
		}

        GameSession? client = player.getClient();
		if (client == null || client.IsDetached)
		{
			return new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_MADE_HASTE_WITH_THEIR_TAIL_BETWEEN_THEIR_LEGS_THE_MATCH_HAS_BEEN_CANCELLED);
		}

		// safety precautions
		if (player.inObserverMode())
		{
			return new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_DOES_NOT_MEET_THE_REQUIREMENTS_TO_DO_BATTLE_THE_MATCH_HAS_BEEN_CANCELLED);
		}

		SystemMessagePacket sm;
		if (player.isDead())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_DEAD_AND_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_DOES_NOT_MEET_THE_REQUIREMENTS_TO_DO_BATTLE_THE_MATCH_HAS_BEEN_CANCELLED);
		}
		if (player.isSubClassActive())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_DOES_NOT_MEET_THE_PARTICIPATION_REQUIREMENTS_YOU_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD_BECAUSE_YOU_HAVE_CHANGED_YOUR_CLASS_TO_SUBCLASS);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_DOES_NOT_MEET_THE_REQUIREMENTS_TO_DO_BATTLE_THE_MATCH_HAS_BEEN_CANCELLED);
		}
		if (player.isCursedWeaponEquipped())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_DOES_NOT_MEET_THE_PARTICIPATION_REQUIREMENTS_THE_OWNER_OF_S2_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD);
			sm.Params.addPcName(player);
			sm.Params.addItemName(player.getCursedWeaponEquippedId());
			player.sendPacket(sm);
			return new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_DOES_NOT_MEET_THE_REQUIREMENTS_TO_DO_BATTLE_THE_MATCH_HAS_BEEN_CANCELLED);
		}
		if (!player.isInventoryUnder90(true))
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_CAN_T_PARTICIPATE_IN_THE_OLYMPIAD_BECAUSE_THEIR_INVENTORY_IS_FILLED_FOR_MORE_THAN_80);
			sm.Params.addPcName(player);
			player.sendPacket(sm);
			return new SystemMessagePacket(SystemMessageId.YOUR_OPPONENT_DOES_NOT_MEET_THE_REQUIREMENTS_TO_DO_BATTLE_THE_MATCH_HAS_BEEN_CANCELLED);
		}

		return null;
	}

	protected static bool portPlayerToArena(Participant par, Location loc, int id, Instance instance)
	{
		Player player = par.getPlayer();
		if (player == null || !player.isOnline())
		{
			return false;
		}

		try
		{
			player.setLastLocation();
			if (player.isSitting())
			{
				player.standUp();
			}
			player.setTarget(null);

			player.setOlympiadGameId(id);
			player.setInOlympiadMode(true);
			player.setOlympiadStart(false);
			player.setOlympiadSide(par.getSide());
			player.teleToLocation(loc, instance);
			player.sendPacket(new ExOlympiadModePacket(2));
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
			return false;
		}
		return true;
	}

	protected void removals(Player player, bool removeParty)
	{
		try
		{
			if (player == null)
			{
				return;
			}

			// Remove Buffs
			player.stopAllEffectsExceptThoseThatLastThroughDeath();
			player.getEffectList().stopEffects(info => info.getSkill().IsBlockedInOlympiad, true, true);

			// Remove Clan Skills
			Clan? clan = player.getClan();
			if (clan != null)
			{
				clan.removeSkillEffects(player);
				if (clan.getCastleId() > 0)
				{
					Castle? castle = CastleManager.getInstance().getCastleByOwner(clan);
					if (castle != null)
					{
						castle.removeResidentialSkills(player);
					}
				}
				if (clan.getFortId() > 0)
				{
					Fort? fort = FortManager.getInstance().getFortByOwner(clan);
					if (fort != null)
					{
						fort.removeResidentialSkills(player);
					}
				}
			}
			// Abort casting if player casting
			player.abortAttack();
			player.abortCast();

			// Force the character to be visible
			player.setInvisible(false);

			// Heal Player fully
			player.setCurrentCp(player.getMaxCp());
			player.setCurrentHp(player.getMaxHp());
			player.setCurrentMp(player.getMaxMp());

			// Remove Summon's Buffs
			if (player.hasSummon())
			{
				Summon? pet = player.getPet();
				if (pet != null)
				{
					pet.unSummon(player);
				}

				player.getServitors().Values.ForEach(s =>
				{
					s.stopAllEffectsExceptThoseThatLastThroughDeath();
					s.getEffectList().stopEffects(info => info.getSkill().IsBlockedInOlympiad, true, true);
					s.abortAttack();
					s.abortCast();
				});
			}

			// stop any cubic that has been given by other player.
			player.stopCubicsByOthers();

			// Remove player from his party
			if (removeParty)
			{
				Party? party = player.getParty();
				if (party != null)
				{
					party.removePartyMember(player, PartyMessageType.EXPELLED);
				}
			}
			// Remove Agathion
			if (player.getAgathionId() > 0)
			{
				player.setAgathionId(0);
				player.broadcastUserInfo();
			}

			player.checkItemRestriction();

			// enable skills with cool time <= 15 minutes
			foreach (Skill skill in player.getAllSkills())
			{
				if (skill.ReuseDelay <= TimeSpan.FromMinutes(15))
				{
					player.enableSkill(skill);
				}
			}

			player.sendSkillList();
			player.sendPacket(new SkillCoolTimePacket(player));
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}

	protected void cleanEffects(Player player)
	{
		try
		{
			// prevent players kill each other
			player.setOlympiadStart(false);
			player.setTarget(null);
			player.abortAttack();
			player.abortCast();
			player.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);

			if (player.isDead())
			{
				player.setDead(false);
			}

			player.stopAllEffectsExceptThoseThatLastThroughDeath();
			player.getEffectList().stopEffects(info => info.getSkill().IsBlockedInOlympiad, true, true);
			player.clearSouls();
			player.clearCharges();
			if (player.getAgathionId() > 0)
			{
				player.setAgathionId(0);
			}
			Summon? pet = player.getPet();
			if (pet != null && !pet.isDead())
			{
				pet.setTarget(null);
				pet.abortAttack();
				pet.abortCast();
				pet.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
				pet.stopAllEffectsExceptThoseThatLastThroughDeath();
				pet.getEffectList().stopEffects(info => info.getSkill().IsBlockedInOlympiad, true, true);
			}

			foreach (Summon s in player.getServitors().Values)
			{
				if (!s.isDead())
				{
					s.setTarget(null);
					s.abortAttack();
					s.abortCast();
					s.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
					s.stopAllEffectsExceptThoseThatLastThroughDeath();
					s.getEffectList().stopEffects(info => info.getSkill().IsBlockedInOlympiad, true, true);
				}
			}

			player.setCurrentCp(player.getMaxCp());
			player.setCurrentHp(player.getMaxHp());
			player.setCurrentMp(player.getMaxMp());
			player.getStatus().startHpMpRegeneration();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}

	protected void playerStatusBack(Player player)
	{
		try
		{
			if (player.isTransformed())
			{
				player.untransform();
			}

			player.setInOlympiadMode(false);
			player.setOlympiadStart(false);
			player.setOlympiadSide(-1);
			player.setOlympiadGameId(-1);
			player.sendPacket(new ExOlympiadModePacket(0));

			// Add Clan Skills
			Clan? clan = player.getClan();
			if (clan != null)
			{
				clan.addSkillEffects(player);
				if (clan.getCastleId() > 0)
				{
					Castle? castle = CastleManager.getInstance().getCastleByOwner(clan);
					if (castle != null)
					{
						castle.giveResidentialSkills(player);
					}
				}
				if (clan.getFortId() > 0)
				{
					Fort? fort = FortManager.getInstance().getFortByOwner(clan);
					if (fort != null)
					{
						fort.giveResidentialSkills(player);
					}
				}
				player.sendSkillList();
			}

			// heal again after adding clan skills
			player.setCurrentCp(player.getMaxCp());
			player.setCurrentHp(player.getMaxHp());
			player.setCurrentMp(player.getMaxMp());
			player.getStatus().startHpMpRegeneration();

			if (Config.DualboxCheck.DUALBOX_CHECK_MAX_OLYMPIAD_PARTICIPANTS_PER_IP > 0)
			{
				AntiFeedManager.getInstance().removePlayer(AntiFeedManager.OLYMPIAD_ID, player);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("playerStatusBack(): " + e);
		}
	}

	protected void portPlayerBack(Player player)
	{
		if (player == null)
		{
			return;
		}
		Location3D? loc = player.getLastLocation();
		if (loc != null)
		{
			player.setIsPendingRevive(false);
			player.teleToLocation(new Location(loc.Value, 0));
			player.unsetLastLocation();
		}
	}

	public static void rewardParticipant(Player player, IReadOnlyList<ItemHolder> list)
	{
		if (player == null || !player.isOnline() || list == null)
		{
			return;
		}

		try
		{
			List<ItemInfo> items = new List<ItemInfo>();
			list.ForEach(holder =>
			{
				Item? item = player.getInventory().addItem("Olympiad", holder.Id, holder.Count, player, null);
				if (item == null)
				{
					return;
				}

				items.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
				sm.Params.addItemName(item);
				sm.Params.addLong(holder.Count);
				player.sendPacket(sm);
			});

			InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
			player.sendInventoryUpdate(iu);
		}
		catch (Exception e)
		{
			LOGGER.Error(e);
		}
	}

	public abstract CompetitionType getType();

	public abstract string[] getPlayerNames();

	public abstract bool containsParticipant(int playerId);

	public abstract void sendOlympiadInfo(Creature creature);

	public abstract void broadcastOlympiadInfo(OlympiadStadium stadium);

	public abstract void broadcastPacket<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket;

	public abstract bool needBuffers();

	public abstract bool checkDefaulted();

	public abstract void removals();

	public abstract bool portPlayersToArena(ImmutableArray<Location3D> spawns, Instance instance);

	public abstract void cleanEffects();

	public abstract void portPlayersBack();

	public abstract void playersStatusBack();

	public abstract void clearPlayers();

	public abstract void handleDisconnect(Player player);

	public abstract void resetDamage();

	public abstract void addDamage(Player player, int damage);

	public abstract bool checkBattleStatus();

	public abstract bool haveWinner();

	public abstract void validateWinner(OlympiadStadium stadium);

	protected abstract int getDivider();

	public abstract void healPlayers();

	public abstract void untransformPlayers();

	public abstract void makePlayersInvul();

	public abstract void removePlayersInvul();
}