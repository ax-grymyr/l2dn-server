using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PveMagicalSkillDefenceBonus")]
public sealed class PveMagicalSkillDefenceBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVE_MAGICAL_SKILL_DEFENCE);