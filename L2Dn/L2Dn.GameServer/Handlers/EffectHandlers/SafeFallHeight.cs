using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class SafeFallHeight: AbstractStatEffect
{
	public SafeFallHeight(StatSet @params): base(@params, Stat.FALL)
	{
	}
}