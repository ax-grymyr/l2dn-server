using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Equivalent of DefenceMagicCriticalDamage for physical skills.
/// </summary>
[HandlerName("DefencePhysicalSkillCriticalDamage")]
public sealed class DefencePhysicalSkillCriticalDamage(EffectParameterSet parameters): AbstractStatEffect(parameters,
    Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE, Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD);