using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Bonus damage to immobile targets.
/// </summary>
public sealed class ImmobileDamageBonus(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.IMMOBILE_DAMAGE_BONUS);