using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class IgnoreReduceDamage(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.IGNORE_REDUCE_DAMAGE);