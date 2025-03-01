using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Create Outpost condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanCreateOutpost(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
	{
        Player? player = effector.getActingPlayer();
		if (!effector.isPlayer() || player is null)
			return !value;

		bool canCreateOutpost = !(player.isAlikeDead() || player.isCursedWeaponEquipped() || player.getClan() == null);

		Castle? castle = CastleManager.getInstance().getCastle(player);
		Fort? fort = FortManager.getInstance().getFort(player);
		if (castle == null && fort == null)
		{
			canCreateOutpost = false;
		}

		if ((fort != null && fort.getResidenceId() == 0) || (castle != null && castle.getResidenceId() == 0))
		{
			player.sendMessage("You must be on fort or castle ground to construct an outpost or flag.");
			canCreateOutpost = false;
		}
		else if ((fort != null && !fort.getZone().isActive()) || (castle != null && !castle.getZone().isActive()))
		{
			player.sendMessage("You can only construct an outpost or flag on siege field.");
			canCreateOutpost = false;
		}
		else if (!player.isClanLeader())
		{
			player.sendMessage("You must be a clan leader to construct an outpost or flag.");
			canCreateOutpost = false;
		}
		else if (!player.isInsideZone(ZoneId.HQ))
		{
			player.sendPacket(SystemMessageId.YOU_CAN_T_BUILD_HEADQUARTERS_HERE);
			canCreateOutpost = false;
		}

		return value == canCreateOutpost;
	}
}