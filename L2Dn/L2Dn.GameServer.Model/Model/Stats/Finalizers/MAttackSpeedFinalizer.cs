using L2Dn.GameServer.Model.Actor;
using L2Dn.Model.Enums;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MAttackSpeedFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double baseValue = calcWeaponBaseValue(creature, stat);
		if (Config.CHAMPION_ENABLE && creature.isChampion())
		{
			baseValue *= Config.CHAMPION_SPD_ATK;
		}

		double witBonus = creature.getWIT() > 0 ? BaseStat.WIT.calcBonus(creature) : 1;
		baseValue *= witBonus;
		return validateValue(creature, defaultValue(creature, stat, baseValue), 1,
			creature.isPlayable() ? Config.Character.MAX_MATK_SPEED : double.MaxValue);
	}

	private double defaultValue(Creature creature, Stat stat, double baseValue)
	{
		double mul = Math.Max(creature.getStat().getMul(stat), 0.7);
		double add = creature.getStat().getAdd(stat);
		return baseValue * mul + add + creature.getStat().getMoveTypeValue(stat, creature.getMoveType());
	}
}