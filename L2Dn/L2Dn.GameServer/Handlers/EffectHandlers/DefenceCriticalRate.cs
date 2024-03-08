using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DefenceCriticalRate: AbstractStatEffect
{
	public DefenceCriticalRate(StatSet @params): base(@params, Stat.DEFENCE_CRITICAL_RATE, Stat.DEFENCE_CRITICAL_RATE_ADD)
	{
	}
}