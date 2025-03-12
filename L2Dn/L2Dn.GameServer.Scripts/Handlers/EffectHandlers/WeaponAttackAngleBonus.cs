using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class WeaponAttackAngleBonus(StatSet @params): AbstractStatAddEffect(@params, Stat.WEAPON_ATTACK_ANGLE_BONUS);