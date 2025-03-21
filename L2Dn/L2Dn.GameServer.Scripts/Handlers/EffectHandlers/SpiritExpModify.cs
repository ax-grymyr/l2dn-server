using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("SpiritExpModify")]
public sealed class SpiritExpModify(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.ELEMENTAL_SPIRIT_BONUS_EXP);