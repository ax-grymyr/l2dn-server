using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PvpPhysicalSkillDefenceBonus")]
public sealed class PvpPhysicalSkillDefenceBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVP_PHYSICAL_SKILL_DEFENCE);