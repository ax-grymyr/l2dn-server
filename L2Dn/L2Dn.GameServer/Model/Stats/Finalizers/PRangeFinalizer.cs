using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class PRangeFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		return Stat.defaultValue(creature, stat, calcWeaponBaseValue(creature, stat));
	}
}