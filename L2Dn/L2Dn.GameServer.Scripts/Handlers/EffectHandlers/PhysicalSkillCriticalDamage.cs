using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalSkillCriticalDamage(StatSet @params): AbstractStatEffect(@params,
    Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE, Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD);