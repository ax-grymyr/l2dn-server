using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class RealDamageResist: AbstractStatPercentEffect
{
	public RealDamageResist(StatSet @params): base(@params, Stat.REAL_DAMAGE_RESIST)
	{
	}
}