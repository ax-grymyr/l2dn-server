using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Resist damage while immobile.
/// </summary>
[HandlerStringKey("ImmobileDamageResist")]
public sealed class ImmobileDamageResist(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.IMMOBILE_DAMAGE_RESIST);