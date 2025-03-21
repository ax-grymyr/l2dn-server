using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("MaxMp")]
public sealed class MaxMpSkillCondition: ISkillCondition
{
    private readonly int _amount;

    public MaxMpSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return caster.getMaxMp() >= _amount;
    }
}