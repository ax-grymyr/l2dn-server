using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class LimitMp: AbstractStatEffect
{
	public LimitMp(StatSet @params): base(@params, Stat.MAX_RECOVERABLE_MP)
	{
	}
}