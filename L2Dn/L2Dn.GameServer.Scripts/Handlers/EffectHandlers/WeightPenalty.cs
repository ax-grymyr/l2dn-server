using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("WeightPenalty")]
public sealed class WeightPenalty(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.WEIGHT_PENALTY);