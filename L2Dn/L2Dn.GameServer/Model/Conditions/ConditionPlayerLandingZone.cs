using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerLandingZone.
 * @author kerberos
 */
public class ConditionPlayerLandingZone : Condition
{
	private readonly bool _value;
	
	/**
	 * Instantiates a new condition player landing zone.
	 * @param value the value
	 */
	public ConditionPlayerLandingZone(bool value)
	{
		_value = value;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		return effector.isInsideZone(ZoneId.LANDING) == _value;
	}
}
