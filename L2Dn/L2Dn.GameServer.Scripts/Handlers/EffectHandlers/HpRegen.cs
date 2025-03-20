using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class HpRegen(EffectParameterSet parameters):
    AbstractConditionalHpEffect(parameters, Stat.REGENERATE_HP_RATE);