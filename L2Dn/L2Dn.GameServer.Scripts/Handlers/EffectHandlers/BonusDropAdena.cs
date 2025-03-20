using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("BonusDropAdena")]
public sealed class BonusDropAdena(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.BONUS_DROP_ADENA);