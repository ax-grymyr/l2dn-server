using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("SpiritshotResistance")]
public sealed class SpiritshotResistance(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.SPIRITSHOT_RESISTANCE);