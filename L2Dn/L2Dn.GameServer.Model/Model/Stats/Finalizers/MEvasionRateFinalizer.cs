using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MEvasionRateFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = calcWeaponPlusBaseValue(creature, stat);

		int level = creature.getLevel();
		if (creature.isPlayer())
		{
			// [Square(WIT)] * 3 + level;
			baseValue += Math.Sqrt(creature.getWIT()) * 3 + level * 2;

			// Enchanted helm bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_HEAD);
		}
		else
		{
			// [Square(DEX)] * 6 + level;
			baseValue += Math.Sqrt(creature.getWIT()) * 3 + level * 2;
			if (level > 69)
			{
				baseValue += level - 69 + 2;
			}
		}

		return validateValue(creature, StatUtil.defaultValue(creature, stat, baseValue), double.NegativeInfinity,
			creature.isPlayable() ? Config.MAX_EVASION : double.MaxValue);
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