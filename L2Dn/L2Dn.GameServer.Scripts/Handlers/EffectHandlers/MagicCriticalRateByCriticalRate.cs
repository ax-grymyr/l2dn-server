using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("MagicCriticalRateByCriticalRate")]
public sealed class MagicCriticalRateByCriticalRate(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE);