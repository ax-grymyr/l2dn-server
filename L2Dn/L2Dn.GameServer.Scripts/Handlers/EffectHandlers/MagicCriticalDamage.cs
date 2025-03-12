using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class MagicCriticalDamage(StatSet @params)
    : AbstractStatEffect(@params, Stat.MAGIC_CRITICAL_DAMAGE, Stat.MAGIC_CRITICAL_DAMAGE_ADD);