using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class OpNeedSummonOrPetSkillCondition: ISkillCondition
{
    private readonly Set<int> _npcIds = new();

    public OpNeedSummonOrPetSkillCondition(SkillConditionParameterSet parameters)
    {
        List<int>? npcIds = parameters.GetInt32ListOptional(XmlSkillConditionParameterType.NpcIds);
        if (npcIds != null)
        {
            _npcIds.addAll(npcIds);
        }
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        Summon? pet = caster.getPet();
        if (pet != null && _npcIds.Contains(pet.Id))
        {
            return true;
        }

        foreach (Summon summon in caster.getServitors().Values)
        {
            if (_npcIds.Contains(summon.Id))
            {
                return true;
            }
        }

        return false;
    }
}