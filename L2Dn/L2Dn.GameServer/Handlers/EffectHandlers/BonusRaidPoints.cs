using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class BonusRaidPoints: AbstractStatPercentEffect
{
	public BonusRaidPoints(StatSet @params): base(@params, Stat.BONUS_RAID_POINTS)
	{
	}
}