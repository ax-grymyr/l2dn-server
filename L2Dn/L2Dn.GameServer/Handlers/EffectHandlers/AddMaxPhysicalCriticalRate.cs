using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author fruit
 */
public class AddMaxPhysicalCriticalRate: AbstractStatEffect
{
	public AddMaxPhysicalCriticalRate(StatSet @params): base(@params, Stat.ADD_MAX_PHYSICAL_CRITICAL_RATE)
	{
	}
}