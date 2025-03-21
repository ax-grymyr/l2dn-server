using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("CriticalDamage")]
public sealed class CriticalDamage(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.CRITICAL_DAMAGE, Stat.CRITICAL_DAMAGE_ADD);