using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class ShotsBonusFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = 1;
		Player? player = creature.getActingPlayer();
		if (player != null)
		{
			Item? weapon = player.getActiveWeaponInstance();
            Weapon? weaponItem = weapon?.getWeaponItem();
			if (weaponItem != null && weapon != null && weapon.isEnchanted())
			{
				switch (weaponItem.getItemGrade())
				{
					case ItemGrade.D:
					case ItemGrade.C:
					{
						baseValue += weapon.getEnchantLevel() * 0.4 / 100;
						break;
					}
					case ItemGrade.B:
					{
						baseValue += weapon.getEnchantLevel() * 0.7 / 100;
						break;
					}
					case ItemGrade.A:
					{
						baseValue += weapon.getEnchantLevel() * 1.4 / 100;
						break;
					}
					case ItemGrade.S:
					{
						baseValue += weapon.getEnchantLevel() * 1.6 / 100;
						break;
					}
				}
			}

			if (player.getActiveRubyJewel() != null)
			{
				baseValue += player.getActiveRubyJewel()?.getBonus() ?? 0;
			}
		}

		return StatUtil.defaultValue(creature, stat, baseValue);
	}
}