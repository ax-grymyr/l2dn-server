using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class AttackAttributeAdd: AbstractEffect
{
	private readonly double _amount;
	
	public AttackAttributeAdd(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		Stat stat = Stat.FIRE_POWER;
		AttributeType maxAttribute = AttributeType.FIRE;
		int maxValue = 0;
		
		foreach (AttributeType attribute in AttributeTypeUtil.AttributeTypes)
		{
			int attributeValue = effected.getStat().getAttackElementValue(attribute);
			if ((attributeValue > 0) && (attributeValue > maxValue))
			{
				maxAttribute = attribute;
				maxValue = attributeValue;
			}
		}
		
		switch (maxAttribute)
		{
			case AttributeType.FIRE:
			{
				stat = Stat.FIRE_POWER;
				break;
			}
			case AttributeType.WATER:
			{
				stat = Stat.WATER_POWER;
				break;
			}
			case AttributeType.WIND:
			{
				stat = Stat.WIND_POWER;
				break;
			}
			case AttributeType.EARTH:
			{
				stat = Stat.EARTH_POWER;
				break;
			}
			case AttributeType.HOLY:
			{
				stat = Stat.HOLY_POWER;
				break;
			}
			case AttributeType.DARK:
			{
				stat = Stat.DARK_POWER;
				break;
			}
		}
		
		effected.getStat().mergeAdd(stat, _amount);
	}
}