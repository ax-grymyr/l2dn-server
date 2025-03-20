using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpExistNpcSkillCondition: ISkillCondition
{
    private readonly List<int> _npcIds;
    private readonly int _range;
    private readonly bool _isAround;

    public OpExistNpcSkillCondition(SkillConditionParameterSet parameters)
    {
        _npcIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.NpcIds) ?? [];
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
}