using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("DefenceMagicCriticalRate")]
public sealed class DefenceMagicCriticalRate(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.DEFENCE_MAGIC_CRITICAL_RATE, Stat.DEFENCE_MAGIC_CRITICAL_RATE_ADD);