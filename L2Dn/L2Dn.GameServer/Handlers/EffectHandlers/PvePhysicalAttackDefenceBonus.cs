using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PvePhysicalAttackDefenceBonus: AbstractStatPercentEffect
{
	public PvePhysicalAttackDefenceBonus(StatSet @params): base(@params, Stat.PVE_PHYSICAL_ATTACK_DEFENCE)
	{
	}
}