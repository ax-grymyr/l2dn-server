using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("SoulshotResistance")]
public sealed class SoulshotResistance(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.SOULSHOT_RESISTANCE);