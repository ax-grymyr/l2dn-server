using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model.Enums;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MaxHpFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = creature.getTemplate().getBaseValue(stat, 0);
		if (creature.isPet())
		{
			Pet pet = (Pet) creature;
			baseValue = pet.getPetLevelData().getPetMaxHP();
		}
		else if (creature.isPlayer())
		{
			Player? player = creature.getActingPlayer();
			if (player != null)
			{
				baseValue = player.getTemplate().getBaseHpMax(player.getLevel());
			}
		}

		double conBonus = creature.getCON() > 0 ? BaseStat.CON.calcBonus(creature) : 1;
		baseValue *= conBonus;

		return defaultValue(creature, stat, baseValue);
	}

	private static double defaultValue(Creature creature, Stat stat, double baseValue)
	{
		double mul = creature.getStat().getMul(stat);
		double add = creature.getStat().getAdd(stat);

		double maxHp = mul * baseValue + add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType());
		bool isPlayer = creature.isPlayer();
        Player? player = creature.getActingPlayer();

		Inventory? inv = creature.getInventory();
		if (inv == null)
		{
			if (isPlayer && player != null)
			{
				if (player.isCursedWeaponEquipped())
				{
					return double.MaxValue;
				}

				mul = creature.getStat().getMul(Stat.HP_LIMIT);
				add = creature.getStat().getAdd(Stat.HP_LIMIT);
				return Math.Min(maxHp, Config.Character.MAX_HP * mul + add);
			}
			return maxHp;
		}

		// Add maxHP bonus from items
		foreach (Item item in inv.getPaperdollItems())
		{
			maxHp += item.getTemplate().getStats(stat, 0);

			// Apply enchanted item bonus HP
			if (item.isArmor() && item.isEnchanted())
			{
				long bodyPart = item.getTemplate().getBodyPart();
				if (bodyPart != ItemTemplate.SLOT_NECK && bodyPart != ItemTemplate.SLOT_LR_EAR && bodyPart != ItemTemplate.SLOT_LR_FINGER)
				{
					maxHp += EnchantItemHPBonusData.getInstance().getHPBonus(item);
				}
			}
		}

		double hpLimit;
		if (isPlayer && player != null && !player.isCursedWeaponEquipped())
		{
			mul = creature.getStat().getMul(Stat.HP_LIMIT);
			add = creature.getStat().getAdd(Stat.HP_LIMIT);
			hpLimit = Config.Character.MAX_HP * mul + add;
		}
		else
		{
			hpLimit = double.MaxValue;
		}

		return Math.Min(maxHp, hpLimit);
	}
}