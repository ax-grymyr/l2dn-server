using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author NosBit
 */
public class ConditionUsingSlotType: Condition
{
	private readonly long _mask;
	
	public ConditionUsingSlotType(long mask)
	{
		_mask = mask;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if ((effector == null) || !effector.isPlayer())
		{
			return false;
		}
		
		Weapon activeWeapon = effector.getActiveWeaponItem();
		if (activeWeapon == null)
		{
			return false;
		}
		
		return (activeWeapon.getBodyPart() & _mask) != 0;
	}
}
