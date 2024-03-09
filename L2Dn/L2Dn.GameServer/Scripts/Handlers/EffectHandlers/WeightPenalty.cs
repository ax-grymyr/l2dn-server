using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class WeightPenalty: AbstractStatAddEffect
{
	public WeightPenalty(StatSet @params): base(@params, Stat.WEIGHT_PENALTY)
	{
	}
}