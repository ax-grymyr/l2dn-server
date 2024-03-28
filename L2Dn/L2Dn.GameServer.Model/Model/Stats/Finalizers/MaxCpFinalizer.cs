using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class MaxCpFinalizer : StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		double baseValue = creature.getTemplate().getBaseValue(stat, 0);
		Player player = creature.getActingPlayer();
		if (player != null)
		{
			baseValue = player.getTemplate().getBaseCpMax(player.getLevel());
		}
		
		double conBonus = creature.getCON() > 0 ? BaseStat.CON.calcBonus(creature) : 1;
		baseValue *= conBonus;
		return StatUtil.defaultValue(creature, stat, baseValue);
	}
}
