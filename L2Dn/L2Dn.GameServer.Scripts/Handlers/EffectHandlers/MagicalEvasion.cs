using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("MagicalEvasion")]
public sealed class MagicalEvasion(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.MAGIC_EVASION_RATE);