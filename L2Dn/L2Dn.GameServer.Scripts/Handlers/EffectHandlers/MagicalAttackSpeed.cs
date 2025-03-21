using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("MagicalAttackSpeed")]
public sealed class MagicalAttackSpeed(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.MAGIC_ATTACK_SPEED);