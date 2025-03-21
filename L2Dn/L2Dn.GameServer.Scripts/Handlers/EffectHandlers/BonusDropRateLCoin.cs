using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("BonusDropRateLCoin")]
public sealed class BonusDropRateLCoin(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.BONUS_DROP_RATE_LCOIN);