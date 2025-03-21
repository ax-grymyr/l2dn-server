using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PhysicalSkillCriticalDamage")]
public sealed class PhysicalSkillCriticalDamage(EffectParameterSet parameters): AbstractStatEffect(parameters,
    Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE, Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD);