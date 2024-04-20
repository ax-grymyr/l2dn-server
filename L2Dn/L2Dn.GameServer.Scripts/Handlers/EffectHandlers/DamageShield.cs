using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DamageShield: AbstractStatAddEffect
{
	public DamageShield(StatSet @params): base(@params, Stat.REFLECT_DAMAGE_PERCENT)
	{
	}
}