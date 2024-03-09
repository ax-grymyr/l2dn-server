using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class LimitHp: AbstractStatEffect
{
	public LimitHp(StatSet @params): base(@params, Stat.MAX_RECOVERABLE_HP)
	{
	}
}