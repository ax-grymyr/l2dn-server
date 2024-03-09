using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DamageShield: AbstractStatAddEffect
{
	public DamageShield(StatSet @params): base(@params, Stat.REFLECT_DAMAGE_PERCENT)
	{
	}
}