using L2Dn.GameServer.Model.Actor;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author Sdw
 */
public class VampiricChanceFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);

		double amount = creature.getStat().getValue(Stat.ABSORB_DAMAGE_PERCENT, 0) * 100;
		double vampiricSum = creature.getStat().getVampiricSum();
		return amount > 0 ? StatUtil.defaultValue(creature, stat, Math.Min(1, vampiricSum / amount / 100)) : 0;
	}
}