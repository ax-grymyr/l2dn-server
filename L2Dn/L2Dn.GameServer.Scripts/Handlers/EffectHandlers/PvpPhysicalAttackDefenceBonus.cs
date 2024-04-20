using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PvpPhysicalAttackDefenceBonus: AbstractStatPercentEffect
{
	public PvpPhysicalAttackDefenceBonus(StatSet @params): base(@params, Stat.PVP_PHYSICAL_ATTACK_DEFENCE)
	{
	}
}