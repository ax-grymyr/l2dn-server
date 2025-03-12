using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class CriticalDamage(StatSet @params)
    : AbstractStatEffect(@params, Stat.CRITICAL_DAMAGE, Stat.CRITICAL_DAMAGE_ADD);