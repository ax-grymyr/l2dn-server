using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author dontknowdontcare
 */
public class SpeedLimit: AbstractStatEffect
{
	public SpeedLimit(StatSet @params): base(@params, Stat.SPEED_LIMIT)
	{
	}
}