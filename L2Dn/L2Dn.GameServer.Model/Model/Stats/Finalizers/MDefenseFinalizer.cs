using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Transforms;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MDefenseFinalizer : StatFunction
{
	private static readonly int[] SLOTS =
	{
		Inventory.PAPERDOLL_LFINGER,
		Inventory.PAPERDOLL_RFINGER,
		Inventory.PAPERDOLL_LEAR,
		Inventory.PAPERDOLL_REAR,
		Inventory.PAPERDOLL_NECK
	};
	
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		double baseValue = creature.getTemplate().getBaseValue(stat, 0);
		if (creature.isPet())
		{
			Pet pet = (Pet) creature;
			baseValue = pet.getPetLevelData().getPetMDef();
		}
		baseValue += calcEnchantedItemBonus(creature, stat);
		
		Inventory inv = creature.getInventory();
		if (inv != null)
		{
			foreach (Item item in inv.getPaperdollItems())
			{
				baseValue += item.getTemplate().getStats(stat, 0);
			}
		}
		
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			foreach (int slot in SLOTS)
			{
				if (!player.getInventory().isPaperdollSlotEmpty(slot))
				{
					int defaultStatValue = player.getTemplate().getBaseDefBySlot(slot);
					baseValue -= creature.getTransformation()?.getBaseDefBySlot(player, slot) ?? defaultStatValue;
				}
			}
		}
		else if (creature.isPet() && creature.getInventory().getPaperdollObjectId(Inventory.PAPERDOLL_NECK) != 0)
		{
			baseValue -= 13;
		}
		if (creature.isRaid())
		{
			baseValue *= Config.RAID_MDEFENCE_MULTIPLIER;
		}
		
		double bonus = creature.getMEN() > 0 ? BaseStat.MEN.calcBonus(creature) : 1;
		baseValue *= bonus * creature.getLevelMod();
		return defaultValue(creature, stat, baseValue);
	}
	
	private double defaultValue(Creature creature, Stat stat, double baseValue)
	{
		double mul = Math.Max(creature.getStat().getMul(stat), 0.5);
		double add = creature.getStat().getAdd(stat);
		return Math.Max(baseValue * mul + add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType()), creature.getTemplate().getBaseValue(stat, 0) * 0.2);
	}
}