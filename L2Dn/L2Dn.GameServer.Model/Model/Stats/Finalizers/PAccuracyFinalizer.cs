using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class PAccuracyFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double baseValue = calcWeaponPlusBaseValue(creature, stat);
		
		// [Square(DEX)] * 5 + level + weapon hitbonus;
		int level = creature.getLevel();
		baseValue += Math.Sqrt(creature.getDEX()) * 5 + level;
		if (level > 69)
		{
			baseValue += level - 69;
		}
		if (level > 77)
		{
			baseValue += 1;
		}
		if (level > 80)
		{
			baseValue += 2;
		}
		if (level > 87)
		{
			baseValue += 2;
		}
		if (level > 92)
		{
			baseValue += 1;
		}
		if (level > 97)
		{
			baseValue += 1;
		}
		
		if (creature.isPlayer())
		{
			// Enchanted gloves bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_GLOVES);
		}
		
		return StatUtil.defaultValue(creature, stat, baseValue);
	}
	
	protected override double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
	{
		if (isBlessed)
		{
			return 0.3 * Math.Max(enchantLevel - 3, 0) + 0.3 * Math.Max(enchantLevel - 6, 0);
		}
		return 0.2 * Math.Max(enchantLevel - 3, 0) + 0.2 * Math.Max(enchantLevel - 6, 0);
	}
}