using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class WeaponBonusMAtkMultiplier: AbstractStatPercentEffect
{
	public WeaponBonusMAtkMultiplier(StatSet @params): base(@params, Stat.WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER)
	{
	}
}