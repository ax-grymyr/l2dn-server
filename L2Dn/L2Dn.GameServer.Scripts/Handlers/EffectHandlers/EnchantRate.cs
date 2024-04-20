using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author zarco
 */
public class EnchantRate: AbstractStatAddEffect
{
	public EnchantRate(StatSet @params): base(@params, Stat.ENCHANT_RATE)
	{
	}
}