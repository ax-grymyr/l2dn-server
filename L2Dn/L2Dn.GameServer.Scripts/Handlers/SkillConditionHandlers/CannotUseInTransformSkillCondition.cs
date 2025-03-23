using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("CannotUseInTransform")]
public sealed class CannotUseInTransformSkillCondition: ISkillCondition
{
    private readonly int _transformId;

    public CannotUseInTransformSkillCondition(SkillConditionParameterSet parameters)
    {
        _transformId = parameters.GetInt32(XmlSkillConditionParameterType.TransformId, -1);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return _transformId > 0 ? caster.getTransformationId() != _transformId : !caster.isTransformed();
    }

    public override int GetHashCode() => _transformId;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._transformId);
}