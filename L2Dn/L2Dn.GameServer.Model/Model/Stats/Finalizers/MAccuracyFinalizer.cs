using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MAccuracyFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double baseValue = calcWeaponPlusBaseValue(creature, stat);
		if (creature.isPlayer())
		{
			// Enchanted gloves bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_GLOVES);
		}
		return StatUtil.defaultValue(creature, stat, baseValue + Math.Sqrt(creature.getWIT()) * 3 + creature.getLevel() * 2);
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