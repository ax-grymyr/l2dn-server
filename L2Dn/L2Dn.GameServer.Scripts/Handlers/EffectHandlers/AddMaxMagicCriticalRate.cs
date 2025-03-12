using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AddMaxMagicCriticalRate(StatSet @params)
    : AbstractStatEffect(@params, Stat.ADD_MAX_MAGIC_CRITICAL_RATE);