using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerActiveSkillId.
 * @author DrHouse
 */
public class ConditionPlayerActiveSkillId(int skillId, int skillLevel = -1): Condition
{
    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Skill? knownSkill = effector.getKnownSkill(skillId);
        if (knownSkill is not null)
            return skillLevel == -1 || skillLevel <= knownSkill.getLevel();

        return false;
    }
}