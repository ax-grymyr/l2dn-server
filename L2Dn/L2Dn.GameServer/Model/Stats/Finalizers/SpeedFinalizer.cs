using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class SpeedFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = getBaseSpeed(creature, stat);
		if (creature.isPlayer())
		{
			// Enchanted feet bonus
			baseValue += calcEnchantBodyPart(creature, ItemTemplate.SLOT_FEET);
		}

		byte speedStat = (byte)creature.getStat().getAdd(Stat.STAT_BONUS_SPEED, -1);
		if ((speedStat >= 0) && (speedStat < BaseStat.values().length))
		{
			BaseStat baseStat = BaseStat.values()[speedStat];
			double bonusDex = Math.Max(0, baseStat.calcValue(creature) - 55);
			baseValue += bonusDex;
		}

		double maxSpeed;
		if (creature.isPlayer())
		{
			maxSpeed = Config.MAX_RUN_SPEED + creature.getStat().getValue(Stat.SPEED_LIMIT, 0);
		}
		else if (creature.isSummon())
		{
			maxSpeed = Config.MAX_RUN_SPEED_SUMMON + creature.getStat().getValue(Stat.SPEED_LIMIT, 0);
		}
		else
		{
			maxSpeed = double.MaxValue;
		}

		return validateValue(creature, Stat.defaultValue(creature, stat, baseValue), 1, maxSpeed);
	}

	protected override double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
	{
		if (isBlessed)
		{
			return Math.Max(enchantLevel - 3, 0) + Math.Max(enchantLevel - 6, 0);
		}

		return (0.6 * Math.Max(enchantLevel - 3, 0)) + (0.6 * Math.Max(enchantLevel - 6, 0));
	}

	private double getBaseSpeed(Creature creature, Stat stat)
	{
		double baseValue = calcWeaponPlusBaseValue(creature, stat);
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			if (player.isMounted())
			{
				PetLevelData data = PetDataTable.getInstance()
					.getPetLevelData(player.getMountNpcId(), player.getMountLevel());
				if (data != null)
				{
					baseValue = data.getSpeedOnRide(stat);
					// if level diff with mount >= 10, it decreases move speed by 50%
					if ((player.getMountLevel() - creature.getLevel()) >= 10)
					{
						baseValue /= 2;
					}

					// if mount is hungry, it decreases move speed by 50%
					if (player.isHungry())
					{
						baseValue /= 2;
					}
				}
			}

			baseValue += Config.RUN_SPD_BOOST;
		}

		if (creature.isPlayable() && creature.isInsideZone(ZoneId.SWAMP))
		{
			SwampZone zone = ZoneManager.getInstance().getZone<SwampZone>(creature);
			if (zone != null)
			{
				baseValue *= zone.getMoveBonus();
			}
		}

		return baseValue;
	}
}