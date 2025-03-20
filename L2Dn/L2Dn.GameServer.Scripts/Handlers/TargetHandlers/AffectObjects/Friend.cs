using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectObjects;

/**
 * @author Nik
 */
public class Friend: IAffectObjectHandler
{
	public bool checkAffectedObject(Creature creature, Creature target)
	{
		if (creature == target)
		{
			return true;
		}

		Player? player = creature.getActingPlayer();
		Player? targetPlayer = target.getActingPlayer();
		if (player != null)
		{
			if (targetPlayer != null)
			{
				// Same player.
				if (player == targetPlayer)
				{
					return true;
				}

				if (Config.Character.ALT_COMMAND_CHANNEL_FRIENDS)
				{
					CommandChannel? playerCC = player.getCommandChannel();
					CommandChannel? targetCC = targetPlayer.getCommandChannel();
					if ((playerCC != null) && (targetCC != null) && (playerCC.getLeaderObjectId() == targetCC.getLeaderObjectId()))
					{
						return true;
					}
				}

				// Party (command channel doesn't make you friends).
				Party? party = player.getParty();
				Party? targetParty = targetPlayer.getParty();
				if ((party != null) && (targetParty != null) && (party.getLeaderObjectId() == targetParty.getLeaderObjectId()))
				{
					return true;
				}

				// Arena.
				if (creature.isInsideZone(ZoneId.PVP) && target.isInsideZone(ZoneId.PVP))
				{
					return false;
				}

				// Duel.
				if (player.isInDuel() && targetPlayer.isInDuel() && (player.getDuelId() == targetPlayer.getDuelId()))
				{
					return false;
				}

				// Olympiad.
				if (player.isInOlympiadMode() && targetPlayer.isInOlympiadMode() && (player.getOlympiadGameId() == targetPlayer.getOlympiadGameId()))
				{
					return false;
				}

				// Clan.
				Model.Clans.Clan? clan = player.getClan();
				Model.Clans.Clan? targetClan = targetPlayer.getClan();
				if (clan != null)
				{
					if (clan == targetClan)
					{
						return true;
					}

					// War
					if ((targetClan != null) && clan.isAtWarWith(targetClan) && targetClan.isAtWarWith(clan))
					{
						return false;
					}
				}

				// Alliance.
				if ((player.getAllyId() != 0) && (player.getAllyId() == targetPlayer.getAllyId()))
				{
					return true;
				}

				// Siege.
				if (target.isInsideZone(ZoneId.SIEGE))
				{
					// Players in the same siege side at the same castle are considered friends.
					return player.isSiegeFriend(targetPlayer);
				}

				// By default any neutral non-flagged player is considered a friend.
				return (targetPlayer.getPvpFlag() == PvpFlagStatus.None) && (targetPlayer.getReputation() >= 0);
			}

			// By default any npc that isnt mob is considered friend.
			return !target.isMonster() && !target.isAutoAttackable(player);
		}

		return !target.isAutoAttackable(creature);
	}

	public AffectObject getAffectObjectType()
	{
		return AffectObject.FRIEND;
	}
}