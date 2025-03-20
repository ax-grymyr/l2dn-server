using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerName("OpTargetNpc")]
public sealed class OpTargetNpcSkillCondition: ISkillCondition
{
    private readonly Set<int> _npcIds = new();

    public OpTargetNpcSkillCondition(SkillConditionParameterSet parameters)
    {
        _npcIds.addAll(parameters.GetInt32ListOptional(XmlSkillConditionParameterType.NpcIds) ?? []);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        WorldObject? actualTarget = caster == null || !caster.isPlayer() ? target : caster.getTarget();
        return actualTarget != null && (actualTarget.isNpc() || actualTarget.isDoor()) &&
            _npcIds.Contains(actualTarget.Id);
    }
}