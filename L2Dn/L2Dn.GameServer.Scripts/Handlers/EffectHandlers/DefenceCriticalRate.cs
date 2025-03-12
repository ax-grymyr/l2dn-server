using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DefenceCriticalRate(StatSet @params)
    : AbstractStatEffect(@params, Stat.DEFENCE_CRITICAL_RATE, Stat.DEFENCE_CRITICAL_RATE_ADD);