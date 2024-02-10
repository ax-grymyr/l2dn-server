using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionChangeWeapon.
 * @author nBd
 */
public class ConditionChangeWeapon: Condition
{
	private readonly bool _required;
	
	/**
	 * Instantiates a new condition change weapon.
	 * @param required the required
	 */
	public ConditionChangeWeapon(bool required)
	{
		_required = required;
	}
	
	/**
	 * Test impl.
	 * @return true, if successful
	 */
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effector.getActingPlayer() == null)
		{
			return false;
		}
		
		if (_required)
		{
			Weapon weaponItem = effector.getActiveWeaponItem();
			if (weaponItem == null)
			{
				return false;
			}
			
			if (weaponItem.getChangeWeaponId() == 0)
			{
				return false;
			}
			
			if (effector.getActingPlayer().hasItemRequest())
			{
				return false;
			}
		}
		return true;
	}
}
