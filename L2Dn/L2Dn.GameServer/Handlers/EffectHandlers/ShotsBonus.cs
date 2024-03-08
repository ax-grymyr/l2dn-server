using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class ShotsBonus: AbstractStatEffect
{
	public ShotsBonus(StatSet @params): base(@params, Stat.SHOTS_BONUS)
	{
	}
}