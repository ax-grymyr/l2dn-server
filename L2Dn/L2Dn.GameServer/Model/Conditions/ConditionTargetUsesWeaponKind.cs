using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionTargetUsesWeaponKind.
 * @author mkizub
 */
public class ConditionTargetUsesWeaponKind : Condition
{
	private readonly ItemTypeMask _weaponMask;
	
	/**
	 * Instantiates a new condition target uses weapon kind.
	 * @param weaponMask the weapon mask
	 */
	public ConditionTargetUsesWeaponKind(ItemTypeMask weaponMask)
	{
		_weaponMask = weaponMask;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		if (effected == null)
		{
			return false;
		}
		
		Weapon weapon = effected.getActiveWeaponItem();
		if (weapon == null)
		{
			return false;
		}
		
		return (weapon.getItemMask() & _weaponMask) != ItemTypeMask.Zero;
	}
}