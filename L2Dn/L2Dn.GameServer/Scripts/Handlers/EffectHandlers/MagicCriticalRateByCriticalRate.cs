using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class MagicCriticalRateByCriticalRate: AbstractStatPercentEffect
{
	public MagicCriticalRateByCriticalRate(StatSet @params): base(@params, Stat.MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE)
	{
	}
}