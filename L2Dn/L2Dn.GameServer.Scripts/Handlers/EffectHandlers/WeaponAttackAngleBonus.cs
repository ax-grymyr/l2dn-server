using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("WeaponAttackAngleBonus")]
public sealed class WeaponAttackAngleBonus(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.WEAPON_ATTACK_ANGLE_BONUS);