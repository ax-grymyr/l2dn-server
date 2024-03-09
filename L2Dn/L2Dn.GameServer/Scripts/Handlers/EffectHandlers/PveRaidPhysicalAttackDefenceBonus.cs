using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PveRaidPhysicalAttackDefenceBonus: AbstractStatPercentEffect
{
	public PveRaidPhysicalAttackDefenceBonus(StatSet @params): base(@params, Stat.PVE_RAID_PHYSICAL_ATTACK_DEFENCE)
	{
	}
}