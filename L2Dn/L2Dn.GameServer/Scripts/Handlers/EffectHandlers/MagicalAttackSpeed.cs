using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class MagicalAttackSpeed: AbstractStatEffect
{
	public MagicalAttackSpeed(StatSet @params): base(@params, Stat.MAGIC_ATTACK_SPEED)
	{
	}
}