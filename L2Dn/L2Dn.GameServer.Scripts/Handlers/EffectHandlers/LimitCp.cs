using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class LimitCp: AbstractStatEffect
{
	public LimitCp(StatSet @params): base(@params, Stat.MAX_RECOVERABLE_CP)
	{
	}
}