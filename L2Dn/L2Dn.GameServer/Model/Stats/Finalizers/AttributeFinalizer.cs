using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class AttributeFinalizer: StatFunction
{
	private readonly AttributeType _type;
	private readonly bool _isWeapon;
	
	public AttributeFinalizer(AttributeType type, bool isWeapon)
	{
		_type = type;
		_isWeapon = isWeapon;
	}
	
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double baseValue = creature.getTemplate().getBaseValue(stat, 0);
		if (creature.isPlayable())
		{
			if (_isWeapon)
			{
				Item weapon = creature.getActiveWeaponInstance();
				if (weapon != null)
				{
					AttributeHolder weaponInstanceHolder = weapon.getAttribute(_type);
					if (weaponInstanceHolder != null)
					{
						baseValue += weaponInstanceHolder.getValue();
					}
					
					AttributeHolder weaponHolder = weapon.getTemplate().getAttribute(_type);
					if (weaponHolder != null)
					{
						baseValue += weaponHolder.getValue();
					}
				}
			}
			else
			{
				Inventory inventory = creature.getInventory();
				if (inventory != null)
				{
					foreach (Item item in inventory.getPaperdollItems(x => x.isArmor()))
					{
						AttributeHolder weaponInstanceHolder = item.getAttribute(_type);
						if (weaponInstanceHolder != null)
						{
							baseValue += weaponInstanceHolder.getValue();
						}
						
						AttributeHolder weaponHolder = item.getTemplate().getAttribute(_type);
						if (weaponHolder != null)
						{
							baseValue += weaponHolder.getValue();
						}
					}
				}
			}
		}
		
		return Stat.defaultValue(creature, stat, baseValue);
	}
}