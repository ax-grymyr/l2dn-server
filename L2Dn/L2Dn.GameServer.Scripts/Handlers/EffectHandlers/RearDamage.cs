using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class RearDamage: AbstractStatPercentEffect
{
	public RearDamage(StatSet @params): base(@params, Stat.REAR_DAMAGE_RATE)
	{
	}
}