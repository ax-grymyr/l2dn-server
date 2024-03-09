using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author dontknowdontcare
 */
public class SpeedLimit: AbstractStatEffect
{
	public SpeedLimit(StatSet @params): base(@params, Stat.SPEED_LIMIT)
	{
	}
}