using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("MagicCriticalRate")]
public sealed class MagicCriticalRate(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.MAGIC_CRITICAL_RATE);