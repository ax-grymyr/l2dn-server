using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author fruit
 */
public class AddMaxMagicCriticalRate: AbstractStatEffect
{
	public AddMaxMagicCriticalRate(StatSet @params): base(@params, Stat.ADD_MAX_MAGIC_CRITICAL_RATE)
	{
	}
}