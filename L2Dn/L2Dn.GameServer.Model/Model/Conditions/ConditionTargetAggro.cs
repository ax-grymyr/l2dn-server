using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetAggro.
 * @author mkizub
 */
public sealed class ConditionTargetAggro(bool isAggro): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        if (effected != null)
        {
            if (effected.isMonster())
                return ((Monster)effected).isAggressive() == isAggro;

            if (effected.isPlayer())
                return ((Player)effected).getReputation() < 0;
        }

        return false;
    }
}