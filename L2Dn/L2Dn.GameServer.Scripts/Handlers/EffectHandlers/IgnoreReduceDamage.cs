using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author fruit
 */
public class IgnoreReduceDamage: AbstractStatEffect
{
	public IgnoreReduceDamage(StatSet @params): base(@params, Stat.IGNORE_REDUCE_DAMAGE)
	{
	}
}