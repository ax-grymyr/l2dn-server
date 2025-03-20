using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("HealEffect")]
public sealed class HealEffect(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.HEAL_EFFECT, Stat.HEAL_EFFECT_ADD);