using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpInstantzone")]
public sealed class OpInstantzoneSkillCondition: ISkillCondition
{
    private readonly int _instanceId;

    public OpInstantzoneSkillCondition(SkillConditionParameterSet parameters)
    {
        _instanceId = parameters.GetInt32(XmlSkillConditionParameterType.InstanceId);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Instance? instance = caster.getInstanceWorld();
        return instance != null && instance.getTemplateId() == _instanceId;
    }

    public override int GetHashCode() => _instanceId;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._instanceId);
}