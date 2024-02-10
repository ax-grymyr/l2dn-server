using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class PCriticalRateFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = calcWeaponBaseValue(creature, stat);
		if (creature.isPlayer())
		{
			// Enchanted legs bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_LEGS);
		}

		double dexBonus = creature.getDEX() > 0 ? BaseStat.DEX.calcBonus(creature) : 1;

		double maxPhysicalCritRate;
		if (creature.isPlayable())
		{
			maxPhysicalCritRate = Config.MAX_PCRIT_RATE +
			                      creature.getStat().getValue(Stat.ADD_MAX_PHYSICAL_CRITICAL_RATE, 0);
		}
		else
		{
			maxPhysicalCritRate = double.MaxValue;
		}

		return validateValue(creature, Stat.defaultValue(creature, stat, baseValue * dexBonus * 10), 0,
			maxPhysicalCritRate);
	}

	protected override double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
	{
		if (isBlessed)
		{
			return (0.5 * Math.Max(enchantLevel - 3, 0)) + (0.5 * Math.Max(enchantLevel - 6, 0));
		}

		return (0.34 * Math.Max(enchantLevel - 3, 0)) + (0.34 * Math.Max(enchantLevel - 6, 0));
	}
}