using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PhysicalAttackRange")]
public sealed class PhysicalAttackRange(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.PHYSICAL_ATTACK_RANGE);