using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetAbnormal.
 * @author janiii
 */
public sealed class ConditionTargetAbnormalType(AbnormalType abnormalType): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        return effected != null && effected.hasAbnormalType(abnormalType);
    }
}