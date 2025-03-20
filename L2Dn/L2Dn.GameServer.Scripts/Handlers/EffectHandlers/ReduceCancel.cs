using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("ReduceCancel")]
public sealed class ReduceCancel(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.ATTACK_CANCEL);