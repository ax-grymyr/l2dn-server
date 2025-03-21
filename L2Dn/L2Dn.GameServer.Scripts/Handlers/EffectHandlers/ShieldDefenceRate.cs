using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("ShieldDefenceRate")]
public sealed class ShieldDefenceRate(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.SHIELD_DEFENCE_RATE);