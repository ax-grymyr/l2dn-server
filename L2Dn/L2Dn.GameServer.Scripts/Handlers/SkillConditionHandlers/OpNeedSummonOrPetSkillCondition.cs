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

[HandlerStringKey("OpNeedSummonOrPet")]
public sealed class OpNeedSummonOrPetSkillCondition: ISkillCondition
{
    private readonly FrozenSet<int> _npcIds;

    public OpNeedSummonOrPetSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? npcIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.NpcIds);
        _npcIds = npcIds is null ? FrozenSet<int>.Empty : npcIds.ToFrozenSet();
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Summon? pet = caster.getPet();
        if (pet != null && _npcIds.Contains(pet.Id))
            return true;

        foreach (Summon summon in caster.getServitors().Values)
        {
            if (_npcIds.Contains(summon.Id))
                return true;
        }

        return false;
    }

    public override int GetHashCode() => _npcIds.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._npcIds.GetSetComparable());
}