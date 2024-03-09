using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AdditionalPotionHp: AbstractStatAddEffect
{
	public AdditionalPotionHp(StatSet @params): base(@params, Stat.ADDITIONAL_POTION_HP)
	{
	}
}