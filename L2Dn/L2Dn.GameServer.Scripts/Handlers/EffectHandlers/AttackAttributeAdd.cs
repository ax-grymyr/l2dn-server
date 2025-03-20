using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("AttackAttributeAdd")]
public sealed class AttackAttributeAdd: AbstractEffect
{
    private readonly double _amount;

    public AttackAttributeAdd(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
    }

    public override void Pump(Creature effected, Skill skill)
    {
        Stat stat = Stat.FIRE_POWER;
        AttributeType maxAttribute = AttributeType.FIRE;
        int maxValue = 0;

        foreach (AttributeType attribute in AttributeTypeUtil.AttributeTypes)
        {
            int attributeValue = effected.getStat().getAttackElementValue(attribute);
            if (attributeValue > 0 && attributeValue > maxValue)
            {
                maxAttribute = attribute;
                maxValue = attributeValue;
            }
        }

        stat = maxAttribute switch
        {
            AttributeType.FIRE => Stat.FIRE_POWER,
            AttributeType.WATER => Stat.WATER_POWER,
            AttributeType.WIND => Stat.WIND_POWER,
            AttributeType.EARTH => Stat.EARTH_POWER,
            AttributeType.HOLY => Stat.HOLY_POWER,
            AttributeType.DARK => Stat.DARK_POWER,
            _ => stat,
        };

        effected.getStat().mergeAdd(stat, _amount);
    }

    public override int GetHashCode() => HashCode.Combine(_amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}