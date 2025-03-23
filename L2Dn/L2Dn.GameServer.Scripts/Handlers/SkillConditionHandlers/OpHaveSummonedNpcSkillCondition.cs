using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("OpHaveSummonedNpc")]
public sealed class OpHaveSummonedNpcSkillCondition: ISkillCondition
{
    private readonly int _npcId;

    public OpHaveSummonedNpcSkillCondition(SkillConditionParameterSet parameters)
    {
        _npcId = parameters.GetInt32(XmlSkillConditionParameterType.NpcId);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        foreach (Npc npc in caster.getSummonedNpcs())
        {
            if (npc.Id == _npcId)
                return true;
        }

        return false;
    }

    public override int GetHashCode() => _npcId;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._npcId);
}