using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("AddMaxMagicCriticalRate")]
public sealed class AddMaxMagicCriticalRate(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.ADD_MAX_MAGIC_CRITICAL_RATE);