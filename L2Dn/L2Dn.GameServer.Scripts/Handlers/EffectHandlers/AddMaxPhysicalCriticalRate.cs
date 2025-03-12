using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AddMaxPhysicalCriticalRate(StatSet @params)
    : AbstractStatEffect(@params, Stat.ADD_MAX_PHYSICAL_CRITICAL_RATE);