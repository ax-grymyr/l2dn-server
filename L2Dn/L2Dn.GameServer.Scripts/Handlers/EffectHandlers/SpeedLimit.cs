using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("SpeedLimit")]
public sealed class SpeedLimit(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.SPEED_LIMIT);