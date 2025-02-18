using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Player Can Escape condition implementation.
 * @author Adry_85
 */
public sealed class ConditionPlayerCanEscape(bool value): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        bool canTeleport = true;
        Player? player = effector.getActingPlayer();
        if (player is null)
            canTeleport = false;
        else if (player.isInDuel())
            canTeleport = false;
        else if (player.isControlBlocked())
            canTeleport = false;
        else if (player.isCombatFlagEquipped())
            canTeleport = false;
        else if (player.isFlying() || player.isFlyingMounted())
            canTeleport = false;
        else if (player.isInOlympiadMode())
            canTeleport = false;
        else if (player.isOnEvent())
            canTeleport = false;

        return value == canTeleport;
    }
}