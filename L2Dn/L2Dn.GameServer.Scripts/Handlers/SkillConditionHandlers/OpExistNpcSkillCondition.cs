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

[HandlerStringKey("OpExistNpc")]
public sealed class OpExistNpcSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _npcIds;
    private readonly int _range;
    private readonly bool _isAround;

    public OpExistNpcSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? npcIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.NpcIds);
        _npcIds = npcIds is null ? FrozenSet<int>.Empty : npcIds.ToFrozenSet();
        _range = parameters.GetInt32(XmlSkillConditionParameterType.Range);
        _isAround = parameters.GetBoolean(XmlSkillConditionParameterType.IsAround);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        foreach (Npc npc in World.getInstance().getVisibleObjectsInRange<Npc>(caster, _range))
        {
            if (_npcIds.Contains(npc.Id))
                return _isAround;
        }

        return !_isAround;
    }

    public override int GetHashCode() => HashCode.Combine(_npcIds.GetSetHashCode(), _range, _isAround);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._npcIds.GetSetComparable(), x._range, x._isAround));
}