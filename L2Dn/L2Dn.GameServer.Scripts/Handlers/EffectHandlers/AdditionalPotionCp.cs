using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AdditionalPotionCp(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.ADDITIONAL_POTION_CP);