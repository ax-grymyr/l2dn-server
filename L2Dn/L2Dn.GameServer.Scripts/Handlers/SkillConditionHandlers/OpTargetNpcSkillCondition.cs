using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpTargetNpc")]
public sealed class OpTargetNpcSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _npcIds;

    public OpTargetNpcSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? npcIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.NpcIds);
        _npcIds = npcIds is null ? FrozenSet<int>.Empty : npcIds.ToFrozenSet();
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        WorldObject? actualTarget = caster == null || !caster.isPlayer() ? target : caster.getTarget();
        return actualTarget != null && (actualTarget.isNpc() || actualTarget.isDoor()) &&
            _npcIds.Contains(actualTarget.Id);
    }

    public override int GetHashCode() => _npcIds.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._npcIds.GetSetComparable());
}