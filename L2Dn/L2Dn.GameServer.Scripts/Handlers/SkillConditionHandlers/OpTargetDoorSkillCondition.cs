using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("OpTargetDoor")]
public sealed class OpTargetDoorSkillCondition: ISkillCondition
{
    private readonly Set<int> _doorIds = [];

    public OpTargetDoorSkillCondition(SkillConditionParameterSet parameters)
    {
        _doorIds.addAll(parameters.GetInt32ListOptional(XmlSkillConditionParameterType.DoorIds) ?? []);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return target != null && target.isDoor() && _doorIds.Contains(target.Id);
    }
}