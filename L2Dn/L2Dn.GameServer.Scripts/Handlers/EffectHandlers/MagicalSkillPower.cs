using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("MagicalSkillPower")]
public sealed class MagicalSkillPower(EffectParameterSet parameters):
    AbstractStatEffect(parameters, Stat.MAGICAL_SKILL_POWER, Stat.SKILL_POWER_ADD);