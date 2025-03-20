using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PhysicalEvasion")]
public sealed class PhysicalEvasion(EffectParameterSet parameters):
    AbstractConditionalHpEffect(parameters, Stat.EVASION_RATE);