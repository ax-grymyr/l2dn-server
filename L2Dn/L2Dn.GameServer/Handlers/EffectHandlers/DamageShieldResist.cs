using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DamageShieldResist: AbstractStatAddEffect
{
	public DamageShieldResist(StatSet @params): base(@params, Stat.REFLECT_DAMAGE_PERCENT_DEFENSE)
	{
	}
}