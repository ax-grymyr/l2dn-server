using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MagicCriticalDamage(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.MAGIC_CRITICAL_DAMAGE, Stat.MAGIC_CRITICAL_DAMAGE_ADD);