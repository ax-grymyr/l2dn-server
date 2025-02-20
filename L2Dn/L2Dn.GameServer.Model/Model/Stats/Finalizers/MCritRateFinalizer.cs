using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MCritRateFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double baseValue = calcWeaponPlusBaseValue(creature, stat);
		if (creature.isPlayer())
		{
			// Enchanted legs bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_LEGS);
		}
		
		double physicalBonus = (creature.getStat().getMul(Stat.MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE, 1) - 1) * creature.getStat().getCriticalHit();
		double witBonus = creature.getWIT() > 0 ? BaseStat.WIT.calcBonus(creature) : 1;
		
		double maxMagicalCritRate;
		if (creature.isPlayable())
		{
			maxMagicalCritRate = Config.MAX_MCRIT_RATE + creature.getStat().getValue(Stat.ADD_MAX_MAGIC_CRITICAL_RATE, 0);
		}
		else
		{
			maxMagicalCritRate = double.MaxValue;
		}
		
		return validateValue(creature, StatUtil.defaultValue(creature, stat, baseValue * witBonus * 10 + physicalBonus), 0, maxMagicalCritRate);
	}
	
	protected override double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
	{
		if (isBlessed)
		{
			return 0.5 * Math.Max(enchantLevel - 3, 0) + 0.5 * Math.Max(enchantLevel - 6, 0);
		}
		return 0.34 * Math.Max(enchantLevel - 3, 0) + 0.34 * Math.Max(enchantLevel - 6, 0);
	}
}
