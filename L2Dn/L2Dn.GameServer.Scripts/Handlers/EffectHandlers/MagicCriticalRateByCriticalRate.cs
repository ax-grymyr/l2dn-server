using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MagicCriticalRateByCriticalRate(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.MAGIC_CRITICAL_RATE_BY_CRITICAL_RATE);