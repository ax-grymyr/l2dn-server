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

[HandlerStringKey("OpTargetDoor")]
public sealed class OpTargetDoorSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _doorIds;

    public OpTargetDoorSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? doorIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.DoorIds);
        _doorIds = doorIds is null ? FrozenSet<int>.Empty : doorIds.ToFrozenSet();
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return target != null && target.isDoor() && _doorIds.Contains(target.Id);
    }

    public override int GetHashCode() => _doorIds.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._doorIds.GetSetComparable());
}