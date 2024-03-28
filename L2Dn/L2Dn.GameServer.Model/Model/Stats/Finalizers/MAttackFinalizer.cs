using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MAttackFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double baseValue = calcWeaponBaseValue(creature, stat);
		if (creature.getActiveWeaponInstance() != null)
		{
			baseValue += creature.getStat().getWeaponBonusMAtk();
			baseValue *= creature.getStat().getMul(Stat.WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER, 1);
		}
		
		baseValue += calcEnchantedItemBonus(creature, stat);
		
		if (creature.isPlayer())
		{
			// Enchanted chest bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_CHEST, ItemTemplate.SLOT_FULL_ARMOR);
		}
		
		if (Config.CHAMPION_ENABLE && creature.isChampion())
		{
			baseValue *= Config.CHAMPION_ATK;
		}
		if (creature.isRaid())
		{
			baseValue *= Config.RAID_MATTACK_MULTIPLIER;
		}
		
		// Calculate modifiers Magic Attack
		double physicalBonus = (creature.getStat().getMul(Stat.MAGIC_ATTACK_BY_PHYSICAL_ATTACK, 1) - 1) * creature.getPAtk();
		baseValue *= Math.Pow(BaseStat.INT.calcBonus(creature) * creature.getLevelMod(), 2.2072);
		return validateValue(creature, StatUtil.defaultValue(creature, stat, baseValue + physicalBonus), 0,
			creature.isPlayable() ? Config.MAX_MATK : double.MaxValue);
	}
	
	protected override double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
	{
		if (isBlessed)
		{
			return (2 * Math.Max(enchantLevel - 3, 0)) + (2 * Math.Max(enchantLevel - 6, 0));
		}
		return (1.4 * Math.Max(enchantLevel - 3, 0)) + (1.4 * Math.Max(enchantLevel - 6, 0));
	}
}