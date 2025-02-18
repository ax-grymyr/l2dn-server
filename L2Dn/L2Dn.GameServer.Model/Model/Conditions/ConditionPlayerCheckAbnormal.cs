using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * Condition implementation to verify player's abnormal type and level.
 * @author Zoey76
 */
public class ConditionPlayerCheckAbnormal(AbnormalType type, int level = -1): Condition
{
    protected override bool TestImpl(Creature effector, Creature effected, Skill? skill, ItemTemplate? item)
    {
        if (level == -1)
            return effector.getEffectList().hasAbnormalType(type);

        return effector.getEffectList().hasAbnormalType(type, info => level >= info.getSkill().getAbnormalLevel());
    }
}