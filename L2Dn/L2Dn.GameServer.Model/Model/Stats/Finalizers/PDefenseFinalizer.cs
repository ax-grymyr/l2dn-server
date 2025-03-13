using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class PDefenseFinalizer: StatFunction
{
	private static readonly int[] SLOTS =
	{
		Inventory.PAPERDOLL_CHEST,
		Inventory.PAPERDOLL_LEGS,
		Inventory.PAPERDOLL_HEAD,
		Inventory.PAPERDOLL_FEET,
		Inventory.PAPERDOLL_GLOVES,
		Inventory.PAPERDOLL_UNDER,
		Inventory.PAPERDOLL_CLOAK,
		Inventory.PAPERDOLL_HAIR
	};

	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		double baseValue = creature.getTemplate().getBaseValue(stat, 0);
		if (creature.isPet())
		{
			Pet pet = (Pet)creature;
			baseValue = pet.getPetLevelData().getPetPDef();
		}

		baseValue += calcEnchantedItemBonus(creature, stat);

		Inventory? inv = creature.getInventory();
		if (inv != null)
		{
			foreach (Item item in inv.getPaperdollItems())
			{
				baseValue += item.getTemplate().getStats(stat, 0);
			}

            Player? player = creature.getActingPlayer();
			if (creature.isPlayer() && player != null)
			{
				foreach (int slot in SLOTS)
				{
					if (!inv.isPaperdollSlotEmpty(slot) || //
					    (slot == Inventory.PAPERDOLL_LEGS && !inv.isPaperdollSlotEmpty(Inventory.PAPERDOLL_CHEST) &&
					     inv.getPaperdollItem(Inventory.PAPERDOLL_CHEST)?.getTemplate().getBodyPart() ==
                         ItemTemplate.SLOT_FULL_ARMOR))
					{
						int defaultStatValue = player.getTemplate().getBaseDefBySlot(slot);
						baseValue -= creature.getTransformation()?.getBaseDefBySlot(player, slot) ?? defaultStatValue;
					}
				}
			}
		}

		if (creature.isRaid())
		{
			baseValue *= Config.RAID_PDEFENCE_MULTIPLIER;
		}

		if (creature.getLevel() > 0)
		{
			baseValue *= creature.getLevelMod();
		}

		return defaultValue(creature, stat, baseValue);
	}

	private double defaultValue(Creature creature, Stat stat, double baseValue)
	{
		double mul = Math.Max(creature.getStat().getMul(stat), 0.5);
		double add = creature.getStat().getAdd(stat);
		return Math.Max(baseValue * mul + add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType()),
			creature.getTemplate().getBaseValue(stat, 0) * 0.2);
	}
}