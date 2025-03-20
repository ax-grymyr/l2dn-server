using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class OpAffectedBySkillSkillCondition: ISkillCondition
{
    private readonly int _skillId;
    private readonly int _skillLevel;

    public OpAffectedBySkillSkillCondition(StatSet @params)
    {
        _skillId = @params.getInt("skillId", -1);
        _skillLevel = @params.getInt("skillLevel", -1);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        BuffInfo? buffInfo = caster.getEffectList().getBuffInfoBySkillId(_skillId);
        if (_skillLevel > 0)
        {
            return buffInfo != null && buffInfo.getSkill().Level >= _skillLevel;
        }

        return buffInfo != null;
    }
}