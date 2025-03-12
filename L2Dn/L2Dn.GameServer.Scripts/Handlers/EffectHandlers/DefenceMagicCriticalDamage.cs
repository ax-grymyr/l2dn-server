using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DefenceMagicCriticalDamage(StatSet @params): AbstractStatEffect(@params,
    Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE, Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD);