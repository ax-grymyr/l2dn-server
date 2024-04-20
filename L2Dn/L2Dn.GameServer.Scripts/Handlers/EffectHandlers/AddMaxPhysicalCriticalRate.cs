using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author fruit
 */
public class AddMaxPhysicalCriticalRate: AbstractStatEffect
{
	public AddMaxPhysicalCriticalRate(StatSet @params): base(@params, Stat.ADD_MAX_PHYSICAL_CRITICAL_RATE)
	{
	}
}