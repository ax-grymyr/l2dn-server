using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class FocusEnergy: AbstractStatAddEffect
{
	public FocusEnergy(StatSet @params): base(@params, Stat.MAX_MOMENTUM)
	{
	}
}