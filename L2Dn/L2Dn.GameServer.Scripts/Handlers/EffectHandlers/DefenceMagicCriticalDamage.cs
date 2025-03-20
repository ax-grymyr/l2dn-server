using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("DefenceMagicCriticalDamage")]
public sealed class DefenceMagicCriticalDamage(EffectParameterSet parameters): AbstractStatEffect(parameters,
    Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE, Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD);