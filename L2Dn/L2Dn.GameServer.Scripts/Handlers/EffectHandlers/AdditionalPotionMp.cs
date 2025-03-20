using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("AdditionalPotionMp")]
public sealed class AdditionalPotionMp(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.ADDITIONAL_POTION_MP);