using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class PveRaidPhysicalAttackDamageBonus: AbstractStatPercentEffect
{
	public PveRaidPhysicalAttackDamageBonus(StatSet @params): base(@params, Stat.PVE_RAID_PHYSICAL_ATTACK_DAMAGE)
	{
	}
}