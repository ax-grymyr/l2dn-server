using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class FishingExpSpBonus: AbstractStatPercentEffect
{
	public FishingExpSpBonus(StatSet @params): base(@params, Stat.FISHING_EXP_SP_BONUS)
	{
	}
}