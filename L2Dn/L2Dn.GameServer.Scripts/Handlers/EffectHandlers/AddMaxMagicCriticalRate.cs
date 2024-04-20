using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author fruit
 */
public class AddMaxMagicCriticalRate: AbstractStatEffect
{
	public AddMaxMagicCriticalRate(StatSet @params): base(@params, Stat.ADD_MAX_MAGIC_CRITICAL_RATE)
	{
	}
}