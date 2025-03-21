using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("DefenceCriticalDamage")]
public sealed class DefenceCriticalDamage(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.DEFENCE_CRITICAL_DAMAGE, Stat.DEFENCE_CRITICAL_DAMAGE_ADD);