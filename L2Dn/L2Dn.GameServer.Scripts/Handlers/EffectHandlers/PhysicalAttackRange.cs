using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalAttackRange: AbstractStatEffect
{
	public PhysicalAttackRange(StatSet @params): base(@params, Stat.PHYSICAL_ATTACK_RANGE)
	{
	}
}