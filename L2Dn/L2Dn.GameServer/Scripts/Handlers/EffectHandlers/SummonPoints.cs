using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class SummonPoints: AbstractStatAddEffect
{
	public SummonPoints(StatSet @params): base(@params, Stat.MAX_SUMMON_POINTS)
	{
	}
}