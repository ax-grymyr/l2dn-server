using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerActiveEffectId.
 */
public sealed class ConditionPlayerActiveEffectId(int effectId, int effectLevel = -1): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        BuffInfo? info = effector.getEffectList().getBuffInfoBySkillId(effectId);
        return info != null && (effectLevel == -1 || effectLevel <= info.getSkill().getLevel());
    }
}