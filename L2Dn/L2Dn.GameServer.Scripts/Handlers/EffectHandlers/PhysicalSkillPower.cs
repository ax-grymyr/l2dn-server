using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("PhysicalSkillPower")]
public sealed class PhysicalSkillPower(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.PHYSICAL_SKILL_POWER, Stat.SKILL_POWER_ADD);