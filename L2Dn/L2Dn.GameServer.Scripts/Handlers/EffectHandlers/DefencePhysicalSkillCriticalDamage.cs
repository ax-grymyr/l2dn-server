using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Equivalent of DefenceMagicCriticalDamage for physical skills.
/// </summary>
public sealed class DefencePhysicalSkillCriticalDamage(StatSet @params): AbstractStatEffect(@params,
    Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE, Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD);