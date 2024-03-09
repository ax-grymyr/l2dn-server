using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AdditionalPotionMp: AbstractStatAddEffect
{
	public AdditionalPotionMp(StatSet @params): base(@params, Stat.ADDITIONAL_POTION_MP)
	{
	}
}