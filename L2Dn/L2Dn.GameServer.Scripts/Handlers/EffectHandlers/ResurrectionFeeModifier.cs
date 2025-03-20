using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ResurrectionFeeModifier(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.RESURRECTION_FEE_MODIFIER);