using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.StaticData;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

/**
 * @author Nik
 */
public class NotFriendPc: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
        Player? targetPlayer = target.getActingPlayer();
		if (!target.isPlayer() || targetPlayer is null)
		{
			return false;
		}

		if (creature == target)
		{
			return false;
		}

		Player? player = creature.getActingPlayer();
		if (player != null)
		{
			// Same player.
			if (player == targetPlayer)
			{
				return false;
			}

			// Peace Zone.
			if (target.isInsidePeaceZone(player) && !player.getAccessLevel().allowPeaceAttack())
			{
				return false;
			}

			if (Config.ALT_COMMAND_CHANNEL_FRIENDS)
			{
				CommandChannel? playerCC = player.getCommandChannel();
				CommandChannel? targetCC = targetPlayer.getCommandChannel();
				if ((playerCC != null) && (targetCC != null) && (playerCC.getLeaderObjectId() == targetCC.getLeaderObjectId()))
				{
					return false;
				}
			}

			// Party (command channel doesn't make you friends).
			Party? party = player.getParty();
			Party? targetParty = targetPlayer.getParty();
			if ((party != null) && (targetParty != null) && (party.getLeaderObjectId() == targetParty.getLeaderObjectId()))
			{
				return false;
			}

			// Events.
			if (player.isOnEvent() && !player.isOnSoloEvent() && (player.getTeam() == target.getTeam()))
			{
				return false;
			}

			// Olympiad observer.
			if (targetPlayer.inObserverMode())
			{
				return false;
			}

			// Siege.
			if (target.isInsideZone(ZoneId.SIEGE))
			{
				// Players in the same siege side at the same castle are considered friends.
				return !player.isSiegeFriend(targetPlayer);
			}

			// Arena.
			if (creature.isInsideZone(ZoneId.PVP) && !creature.isInsideZone(ZoneId.SIEGE) && target.isInsideZone(ZoneId.PVP) && !target.isInsideZone(ZoneId.SIEGE))
			{
				return true;
			}

			// Duel.
			if (player.isInDuel() && targetPlayer.isInDuel() && (player.getDuelId() == targetPlayer.getDuelId()))
			{
				return true;
			}

			// Olympiad.
			if (player.isInOlympiadMode() && targetPlayer.isInOlympiadMode() && (player.getOlympiadGameId() == targetPlayer.getOlympiadGameId()))
			{
				return true;
			}

			// Clan.
			Model.Clans.Clan? clan = player.getClan();
			Model.Clans.Clan? targetClan = targetPlayer.getClan();
			if (clan != null)
			{
				if (clan == targetClan)
				{
					return false;
				}

				// War
				if ((targetClan != null) && clan.isAtWarWith(targetClan) && targetClan.isAtWarWith(clan))
				{
					return true;
				}
			}

			// Alliance.
			if ((player.getAllyId() != 0) && (player.getAllyId() == targetPlayer.getAllyId()))
			{
				return false;
			}

			// Auto play target mode check.
			if (player.isAutoPlaying() && ((targetPlayer.getPvpFlag() == PvpFlagStatus.None) || (targetPlayer.getReputation() > -1)))
			{
				int targetMode = player.getAutoPlaySettings().getNextTargetMode();
				if ((targetMode != 0 /* Any Target */) && (targetMode != 2 /* Characters */))
				{
					return false;
				}
			}

			// By default any flagged/PK player is considered enemy.
			return (targetPlayer.getPvpFlag() != PvpFlagStatus.None) || (targetPlayer.getReputation() < 0);
		}

		return target.isAutoAttackable(creature);
	}

	public AffectObject getAffectObjectType()
	{
		return AffectObject.NOT_FRIEND_PC;
	}
}