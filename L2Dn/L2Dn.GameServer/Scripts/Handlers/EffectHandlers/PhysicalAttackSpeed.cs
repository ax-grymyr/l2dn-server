using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalAttackSpeed: AbstractStatEffect
{
	public PhysicalAttackSpeed(StatSet @params): base(@params, Stat.PHYSICAL_ATTACK_SPEED)
	{
	}
}