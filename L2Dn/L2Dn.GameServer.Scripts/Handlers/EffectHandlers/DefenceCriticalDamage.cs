using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DefenceCriticalDamage(StatSet @params)
    : AbstractStatEffect(@params, Stat.DEFENCE_CRITICAL_DAMAGE, Stat.DEFENCE_CRITICAL_DAMAGE_ADD);