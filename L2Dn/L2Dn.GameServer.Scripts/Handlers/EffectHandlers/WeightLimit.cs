using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("WeightLimit")]
public sealed class WeightLimit(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.WEIGHT_LIMIT);