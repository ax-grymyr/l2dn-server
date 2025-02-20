using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpSkillAcquireSkillCondition: ISkillCondition
{
    private readonly int _skillId;
    private readonly bool _hasLearned;

    public OpSkillAcquireSkillCondition(StatSet @params)
    {
        _skillId = @params.getInt("skillId");
        _hasLearned = @params.getBoolean("hasLearned");
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        if (target == null || !target.isCreature())
        {
            return false;
        }

        int skillLevel = ((Creature)target).getSkillLevel(_skillId);
        return _hasLearned ? skillLevel != 0 : skillLevel == 0;
    }
}