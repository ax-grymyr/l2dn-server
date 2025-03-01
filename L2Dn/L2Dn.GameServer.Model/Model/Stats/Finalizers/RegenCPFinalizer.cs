using L2Dn.GameServer.Model.Actor;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class RegenCPFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
        Player? player = creature.getActingPlayer();
		if (!creature.isPlayer() || player == null)
		{
			return 0;
		}

		double baseValue = player.getTemplate().getBaseCpRegen(creature.getLevel()) * creature.getLevelMod() *
		                   BaseStat.CON.calcBonus(creature) * Config.CP_REGEN_MULTIPLIER;
		if (player.isSitting())
		{
			baseValue *= 1.5; // Sitting
		}
		else if (!player.isMoving())
		{
			baseValue *= 1.1; // Staying
		}
		else if (player.isRunning())
		{
			baseValue *= 0.7; // Running
		}

		return StatUtil.defaultValue(player, stat, baseValue);
	}
}