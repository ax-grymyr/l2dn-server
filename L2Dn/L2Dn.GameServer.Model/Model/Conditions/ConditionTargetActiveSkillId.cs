using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetActiveSkillId.
 */
public class ConditionTargetActiveSkillId(int skillId, int skillLevel = -1): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        if (effected is null)
            return false;

        Skill? knownSkill = effected.getKnownSkill(skillId);
        if (knownSkill != null)
            return skillLevel == -1 || skillLevel <= knownSkill.getLevel();

        return false;
    }
}