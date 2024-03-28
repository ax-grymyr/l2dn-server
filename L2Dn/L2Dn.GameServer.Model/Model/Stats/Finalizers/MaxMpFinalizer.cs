using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MaxMpFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double baseValue = creature.getTemplate().getBaseValue(stat, 0);
		if (creature.isPet())
		{
			Pet pet = (Pet) creature;
			baseValue = pet.getPetLevelData().getPetMaxMP();
		}
		else if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			if (player != null)
			{
				baseValue = player.getTemplate().getBaseMpMax(player.getLevel());
			}
		}
		
		double menBonus = creature.getMEN() > 0 ? BaseStat.MEN.calcBonus(creature) : 1;
		baseValue *= menBonus;
		
		return defaultValue(creature, stat, baseValue);
	}
	
	private static double defaultValue(Creature creature, Stat stat, double baseValue)
	{
		double mul = creature.getStat().getMul(stat);
		double add = creature.getStat().getAdd(stat);
		double addItem = 0;
		
		Inventory inv = creature.getInventory();
		if (inv != null)
		{
			// Add maxMP bonus from items
			foreach (Item item in inv.getPaperdollItems())
			{
				addItem += item.getTemplate().getStats(stat, 0);
			}
		}
		
		return (mul * baseValue) + add + addItem + creature.getStat().getMoveTypeValue(stat, creature.getMoveType());
	}
}