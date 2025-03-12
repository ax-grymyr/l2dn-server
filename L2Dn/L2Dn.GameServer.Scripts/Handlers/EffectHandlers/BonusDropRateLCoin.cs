using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class BonusDropRateLCoin(StatSet @params): AbstractStatPercentEffect(@params, Stat.BONUS_DROP_RATE_LCOIN);