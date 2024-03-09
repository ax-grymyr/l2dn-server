using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class WeightLimit: AbstractStatEffect
{
	public WeightLimit(StatSet @params): base(@params, Stat.WEIGHT_LIMIT)
	{
	}
}