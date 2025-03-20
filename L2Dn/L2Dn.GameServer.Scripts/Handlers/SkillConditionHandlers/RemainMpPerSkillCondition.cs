using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

public sealed class RemainMpPerSkillCondition: ISkillCondition
{
    private readonly int _amount;
    private readonly SkillConditionPercentType _percentType;

    public RemainMpPerSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
        _percentType = parameters.GetEnum<SkillConditionPercentType>(XmlSkillConditionParameterType.PercentType);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return _percentType.test(caster.getCurrentMpPercent(), _amount);
    }
}