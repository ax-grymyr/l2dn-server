using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class TargetNotAffectedBySkillSkillCondition: ISkillCondition
{
    private readonly int _skillId;
    private readonly int _skillLevel;

    public TargetNotAffectedBySkillSkillCondition(StatSet @params)
    {
        _skillId = @params.getInt("skillId", -1);
        _skillLevel = @params.getInt("skillLevel", -1);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isCreature())
        {
            return false;
        }

        BuffInfo? buffInfo = ((Creature)target).getEffectList().getBuffInfoBySkillId(_skillId);
        if (_skillLevel > 0)
        {
            return buffInfo == null || buffInfo.getSkill().Level < _skillLevel;
        }

        return buffInfo == null;
    }
}