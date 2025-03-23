using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpNotAffectedBySkill")]
public sealed class OpNotAffectedBySkillSkillCondition: ISkillCondition
{
    private readonly int _skillId;
    private readonly int _skillLevel;

    public OpNotAffectedBySkillSkillCondition(SkillConditionParameterSet parameters)
    {
        _skillId = parameters.GetInt32(XmlSkillConditionParameterType.SkillId, -1);
        _skillLevel = parameters.GetInt32(XmlSkillConditionParameterType.SkillLevel, -1);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        BuffInfo? buffInfo = caster.getEffectList().getBuffInfoBySkillId(_skillId);
        if (_skillLevel > 0)
        {
            return buffInfo == null || buffInfo.getSkill().Level < _skillLevel;
        }

        return buffInfo == null;
    }

    public override int GetHashCode() => HashCode.Combine(_skillId, _skillLevel);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._skillId, x._skillLevel));
}