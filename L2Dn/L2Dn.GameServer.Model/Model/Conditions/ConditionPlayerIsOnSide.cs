using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerIsOnSide.
 * @author St3eT
 */
public sealed class ConditionPlayerIsOnSide(CastleSide side): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player is null)
            return false;

        return player.getPlayerSide() == side;
    }
}