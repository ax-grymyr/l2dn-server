using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class FatalBlowRate: AbstractStatEffect
{
	public FatalBlowRate(StatSet @params): base(@params, Stat.BLOW_RATE)
	{
	}
}