using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author Sdw
 */
public class ShieldDefenceRateFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		return Stat.defaultValue(creature, stat, calcWeaponPlusBaseValue(creature, stat));
	}
}