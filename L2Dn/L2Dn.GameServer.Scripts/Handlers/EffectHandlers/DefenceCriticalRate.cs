using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("DefenceCriticalRate")]
public sealed class DefenceCriticalRate(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.DEFENCE_CRITICAL_RATE, Stat.DEFENCE_CRITICAL_RATE_ADD);