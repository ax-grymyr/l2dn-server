using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class WeaponBonusPAtkMultiplier: AbstractStatPercentEffect
{
	public WeaponBonusPAtkMultiplier(StatSet @params): base(@params, Stat.WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER)
	{
	}
}