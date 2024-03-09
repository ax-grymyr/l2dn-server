using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class VitalityPointsRate: AbstractStatPercentEffect
{
	public VitalityPointsRate(StatSet @params): base(@params, Stat.VITALITY_CONSUME_RATE)
	{
	}
}