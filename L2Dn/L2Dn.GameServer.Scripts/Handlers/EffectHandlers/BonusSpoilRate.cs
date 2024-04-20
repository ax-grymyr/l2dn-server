using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class BonusSpoilRate: AbstractStatPercentEffect
{
	public BonusSpoilRate(StatSet @params): base(@params, Stat.BONUS_SPOIL_RATE)
	{
	}
}