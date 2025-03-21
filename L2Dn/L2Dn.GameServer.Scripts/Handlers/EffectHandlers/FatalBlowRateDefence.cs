using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("FatalBlowRateDefence")]
public sealed class FatalBlowRateDefence(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.BLOW_RATE_DEFENCE);