using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class BonusDropRateLCoin: AbstractStatPercentEffect
{
	public BonusDropRateLCoin(StatSet @params): base(@params, Stat.BONUS_DROP_RATE_LCOIN)
	{
	}
}