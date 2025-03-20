using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("FishingExpSpBonus")]
public sealed class FishingExpSpBonus(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.FISHING_EXP_SP_BONUS);