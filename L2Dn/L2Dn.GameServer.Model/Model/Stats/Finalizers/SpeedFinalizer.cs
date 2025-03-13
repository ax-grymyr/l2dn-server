using System.Collections.Immutable;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Model.Zones.Types;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

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

		int speedStat = (int)creature.getStat().getAdd(Stat.STAT_BONUS_SPEED, -1);
		ImmutableArray<BaseStat> baseStats = EnumUtil.GetValues<BaseStat>();
		if (speedStat >= 0 && speedStat < baseStats.Length)
		{
			BaseStat baseStat = baseStats[speedStat];
			double bonusDex = Math.Max(0, baseStat.calcValue(creature) - 55);
			baseValue += bonusDex;
		}

		double maxSpeed;
		if (creature.isPlayer())
		{
			maxSpeed = Config.Character.MAX_RUN_SPEED + creature.getStat().getValue(Stat.SPEED_LIMIT, 0);
		}
		else if (creature.isSummon())
		{
			maxSpeed = Config.Character.MAX_RUN_SPEED_SUMMON + creature.getStat().getValue(Stat.SPEED_LIMIT, 0);
		}
		else
		{
			maxSpeed = double.MaxValue;
		}

		return validateValue(creature, StatUtil.defaultValue(creature, stat, baseValue), 1, maxSpeed);
	}

	protected override double calcEnchantBodyPartBonus(int enchantLevel, bool isBlessed)
	{
		if (isBlessed)
		{
			return Math.Max(enchantLevel - 3, 0) + Math.Max(enchantLevel - 6, 0);
		}

		return 0.6 * Math.Max(enchantLevel - 3, 0) + 0.6 * Math.Max(enchantLevel - 6, 0);
	}

	private double getBaseSpeed(Creature creature, Stat stat)
	{
		double baseValue = calcWeaponPlusBaseValue(creature, stat);
        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null)
		{
			if (player.isMounted())
			{
				PetLevelData? data = PetDataTable.getInstance()
					.getPetLevelData(player.getMountNpcId(), player.getMountLevel());
				if (data != null)
				{
					baseValue = data.getSpeedOnRide(stat);
					// if level diff with mount >= 10, it decreases move speed by 50%
					if (player.getMountLevel() - creature.getLevel() >= 10)
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

			baseValue += Config.Character.RUN_SPD_BOOST;
		}

		if (creature.isPlayable() && creature.isInsideZone(ZoneId.SWAMP))
		{
			SwampZone? zone = ZoneManager.getInstance().getZone<SwampZone>(creature.Location.Location3D);
			if (zone != null)
			{
				baseValue *= zone.getMoveBonus();
			}
		}

		return baseValue;
	}
}