using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author zarco
 */
public class EnchantRate: AbstractStatAddEffect
{
	public EnchantRate(StatSet @params): base(@params, Stat.ENCHANT_RATE)
	{
	}
}