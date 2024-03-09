using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PvePhysicalAttackDamageBonus: AbstractStatPercentEffect
{
	public PvePhysicalAttackDamageBonus(StatSet @params): base(@params, Stat.PVE_PHYSICAL_ATTACK_DAMAGE)
	{
	}
}