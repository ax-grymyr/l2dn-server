using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class CubicMastery: AbstractStatAddEffect
{
	public CubicMastery(StatSet @params): base(@params, Stat.MAX_CUBIC)
	{
	}
}