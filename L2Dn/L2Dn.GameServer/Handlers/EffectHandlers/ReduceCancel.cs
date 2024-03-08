using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ReduceCancel: AbstractStatEffect
{
	public ReduceCancel(StatSet @params): base(@params, Stat.ATTACK_CANCEL)
	{
	}
}