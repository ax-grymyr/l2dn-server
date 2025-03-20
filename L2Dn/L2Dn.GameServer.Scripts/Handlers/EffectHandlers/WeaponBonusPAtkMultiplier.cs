using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("WeaponBonusPAtkMultiplier")]
public sealed class WeaponBonusPAtkMultiplier(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.WEAPON_BONUS_PHYSICAL_ATTACK_MULTIPIER);