using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("SkillPowerAdd")]
public sealed class SkillPowerAdd(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.SKILL_POWER_ADD);