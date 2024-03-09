using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AdditionalPotionCp: AbstractStatAddEffect
{
	public AdditionalPotionCp(StatSet @params): base(@params, Stat.ADDITIONAL_POTION_CP)
	{
	}
}