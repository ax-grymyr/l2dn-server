using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerAgathionId.
 */
public sealed class ConditionPlayerAgathionId(int agathionId): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? actingPlayer = effector.getActingPlayer();
        return actingPlayer is not null && actingPlayer.getAgathionId() == agathionId;
    }
}