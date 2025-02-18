using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetAbnormal.
 * @author janiii
 */
public sealed class ConditionTargetAbnormalType(AbnormalType abnormalType): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        return effected.hasAbnormalType(abnormalType);
    }
}