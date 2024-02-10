using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * @author NosBit
 */
public class ConditionUsingSlotType: Condition
{
	private readonly int _mask;
	
	public ConditionUsingSlotType(int mask)
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
