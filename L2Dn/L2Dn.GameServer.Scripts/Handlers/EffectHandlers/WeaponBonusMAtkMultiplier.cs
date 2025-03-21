using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("WeaponBonusMAtkMultiplier")]
public sealed class WeaponBonusMAtkMultiplier(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.WEAPON_BONUS_MAGIC_ATTACK_MULTIPIER);