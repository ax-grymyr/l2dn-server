using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("AddMaxPhysicalCriticalRate")]
public sealed class AddMaxPhysicalCriticalRate(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.ADD_MAX_PHYSICAL_CRITICAL_RATE);