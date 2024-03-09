using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Bonus damage to immobile targets.
 * @author Mobius
 */
public class ImmobileDamageBonus: AbstractStatPercentEffect
{
	public ImmobileDamageBonus(StatSet @params): base(@params, Stat.IMMOBILE_DAMAGE_BONUS)
	{
	}
}