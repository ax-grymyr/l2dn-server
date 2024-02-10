using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author Mobius
 */
public class MpVampiricChanceFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double amount = creature.getStat().getValue(Stat.ABSORB_MANA_DAMAGE_PERCENT, 0) * 100;
		double mpVampiricSum = creature.getStat().getMpVampiricSum();
		return amount > 0 ? Stat.defaultValue(creature, stat, Math.Min(1, mpVampiricSum / amount / 100)) : 0;
	}
}