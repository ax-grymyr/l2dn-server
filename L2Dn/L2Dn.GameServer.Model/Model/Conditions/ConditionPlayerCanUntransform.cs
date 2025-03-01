using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Untransform condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanUntransform(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        bool canUntransform = true;
        Player? player = effector.getActingPlayer();
        if (player is null)
        {
            canUntransform = false;
        }
        else if (player.isAlikeDead() || player.isCursedWeaponEquipped())
        {
            canUntransform = false;
        }
        else if (player.isFlyingMounted() && !player.isInsideZone(ZoneId.LANDING))
        {
            player.sendPacket(SystemMessageId.
                YOU_ARE_TOO_HIGH_TO_PERFORM_THIS_ACTION_PLEASE_LOWER_YOUR_ALTITUDE_AND_TRY_AGAIN); // TODO: check if message is retail like.

            canUntransform = false;
        }

        return value == canUntransform;
    }
}