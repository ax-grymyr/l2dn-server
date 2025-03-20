using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpSkillAcquireSkillCondition: ISkillCondition
{
    private readonly int _skillId;
    private readonly bool _hasLearned;

    public OpSkillAcquireSkillCondition(SkillConditionParameterSet parameters)
    {
        _skillId = parameters.GetInt32(XmlSkillConditionParameterType.SkillId);
        _hasLearned = parameters.GetBoolean(XmlSkillConditionParameterType.HasLearned);
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