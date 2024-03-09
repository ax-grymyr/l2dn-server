using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class HitNumber: AbstractStatEffect
{
	public HitNumber(StatSet @params): base(@params, Stat.ATTACK_COUNT_MAX)
	{
	}
}