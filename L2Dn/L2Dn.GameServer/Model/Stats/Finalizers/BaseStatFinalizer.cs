using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;

namespace L2Dn.GameServer.Model.Stats.Finalizers;

/**
 * @author UnAfraid
 */
public class BaseStatFinalizer: StatFunction
{
	public override double calc(Creature creature, double? @base, Stat stat)
	{
		throwIfPresent(@base);
		
		// Apply template value
		double baseValue = creature.getTemplate().getBaseValue(stat, 0);
		
		// Should not apply armor set and henna bonus to summons.
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			
			// Armor sets calculation
			baseValue += player.getInventory().getPaperdollCache().getBaseStatValue(player, (BaseStat)stat);
			
			// Henna calculation
			baseValue += player.getHennaValue((BaseStat)stat);
			
			// Bonus stats
			switch (stat)
			{
				case Stat.STAT_STR:
				{
					baseValue += player.getVariables().getInt(PlayerVariables.STAT_STR, 0);
					break;
				}
				case Stat.STAT_CON:
				{
					baseValue += player.getVariables().getInt(PlayerVariables.STAT_CON, 0);
					break;
				}
				case Stat.STAT_DEX:
				{
					baseValue += player.getVariables().getInt(PlayerVariables.STAT_DEX, 0);
					break;
				}
				case Stat.STAT_INT:
				{
					baseValue += player.getVariables().getInt(PlayerVariables.STAT_INT, 0);
					break;
				}
				case Stat.STAT_MEN:
				{
					baseValue += player.getVariables().getInt(PlayerVariables.STAT_MEN, 0);
					break;
				}
				case Stat.STAT_WIT:
				{
					baseValue += player.getVariables().getInt(PlayerVariables.STAT_WIT, 0);
					break;
				}
			}
		}
		
		return validateValue(creature, StatUtil.defaultValue(creature, stat, baseValue), 1, BaseStatUtil.MAX_STAT_VALUE - 1);
	}
}