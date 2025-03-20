using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DefencePhysicalSkillCriticalRate(EffectParameterSet parameters): AbstractStatEffect(parameters,
    Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE, Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD);