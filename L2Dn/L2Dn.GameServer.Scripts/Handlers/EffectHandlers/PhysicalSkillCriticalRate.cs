using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PhysicalSkillCriticalRate")]
public sealed class PhysicalSkillCriticalRate(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.CRITICAL_RATE_SKILL);