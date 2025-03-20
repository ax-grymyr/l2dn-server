using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("VitalityPointsRate")]
public sealed class VitalityPointsRate(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.VITALITY_CONSUME_RATE);