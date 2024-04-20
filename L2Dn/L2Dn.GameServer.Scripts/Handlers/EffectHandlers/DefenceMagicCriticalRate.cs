using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DefenceMagicCriticalRate: AbstractStatEffect
{
	public DefenceMagicCriticalRate(StatSet @params): base(@params, Stat.DEFENCE_MAGIC_CRITICAL_RATE, Stat.DEFENCE_MAGIC_CRITICAL_RATE_ADD)
	{
	}
}