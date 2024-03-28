using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author NasSeKa
 */
public class WeaponAttackAngleBonus: AbstractStatAddEffect
{
	public WeaponAttackAngleBonus(StatSet @params): base(@params, Stat.WEAPON_ATTACK_ANGLE_BONUS)
	{
	}
}