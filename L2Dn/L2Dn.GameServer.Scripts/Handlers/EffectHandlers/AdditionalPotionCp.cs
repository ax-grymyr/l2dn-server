using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("AdditionalPotionCp")]
public sealed class AdditionalPotionCp(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.ADDITIONAL_POTION_CP);