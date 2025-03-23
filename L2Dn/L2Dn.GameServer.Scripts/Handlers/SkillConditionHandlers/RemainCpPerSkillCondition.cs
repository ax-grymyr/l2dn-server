using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

[HandlerStringKey("RemainCpPer")]
public sealed class RemainCpPerSkillCondition: ISkillCondition
{
    private readonly int _amount;
    private readonly SkillConditionPercentType _percentType;

    public RemainCpPerSkillCondition(SkillConditionParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillConditionParameterType.Amount);
        _percentType = parameters.GetEnum<SkillConditionPercentType>(XmlSkillConditionParameterType.PercentType);
    }

    public bool canUse(Creature caster, Skill skill, WorldObject? target)
    {
        return _percentType.test(caster.getCurrentCpPercent(), _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _percentType);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._percentType));
}