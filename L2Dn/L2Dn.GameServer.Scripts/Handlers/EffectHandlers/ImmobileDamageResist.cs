using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Resist damage while immobile.
 * @author Mobius
 */
public class ImmobileDamageResist: AbstractStatPercentEffect
{
	public ImmobileDamageResist(StatSet @params): base(@params, Stat.IMMOBILE_DAMAGE_RESIST)
	{
	}
}