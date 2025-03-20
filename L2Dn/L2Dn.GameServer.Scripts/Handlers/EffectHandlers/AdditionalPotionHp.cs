using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("AdditionalPotionHp")]
public sealed class AdditionalPotionHp(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.ADDITIONAL_POTION_HP);