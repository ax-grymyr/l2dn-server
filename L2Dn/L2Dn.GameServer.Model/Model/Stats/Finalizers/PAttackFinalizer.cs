using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.Model.Enums;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class PAttackFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = calcWeaponBaseValue(creature, stat);
		if (creature.getActiveWeaponInstance() != null)
		{
			baseValue += creature.getStat().getWeaponBonusPAtk();
			baseValue *= creature.getStat().getMul(Stat.WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER, 1);
		}

		baseValue += calcEnchantedItemBonus(creature, stat);

		if (creature.isPlayer())
		{
			// Enchanted chest bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_CHEST, ItemTemplate.SLOT_FULL_ARMOR);
		}

		if (Config.ChampionMonsters.CHAMPION_ENABLE && creature.isChampion())
		{
			baseValue *= Config.ChampionMonsters.CHAMPION_ATK;
		}
		if (creature.isRaid())
		{
			baseValue *= Config.Npc.RAID_PATTACK_MULTIPLIER;
		}
		baseValue *= BaseStat.STR.calcBonus(creature) * creature.getLevelMod();
		return validateValue(creature, StatUtil.defaultValue(creature, stat, baseValue), 0, creature.isPlayable() ? Config.Character.MAX_PATK : double.MaxValue);
	}

	protected override double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
	{
		if (isBlessed)
		{
			return 3 * Math.Max(enchantLevel - 3, 0) + 3 * Math.Max(enchantLevel - 6, 0);
		}
		return 2 * Math.Max(enchantLevel - 3, 0) + 2 * Math.Max(enchantLevel - 6, 0);
	}
}