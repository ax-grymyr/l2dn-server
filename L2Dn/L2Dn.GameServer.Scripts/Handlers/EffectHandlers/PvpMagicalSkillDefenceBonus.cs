using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PvpMagicalSkillDefenceBonus")]
public sealed class PvpMagicalSkillDefenceBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVP_MAGICAL_SKILL_DEFENCE);