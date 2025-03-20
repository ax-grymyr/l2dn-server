using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("FatalBlowRate")]
public sealed class FatalBlowRate(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.BLOW_RATE);