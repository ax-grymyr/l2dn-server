using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PhysicalAttackSpeed")]
public sealed class PhysicalAttackSpeed(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.PHYSICAL_ATTACK_SPEED);