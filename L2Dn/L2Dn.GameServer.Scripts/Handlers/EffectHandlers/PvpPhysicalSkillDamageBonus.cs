using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PvpPhysicalSkillDamageBonus")]
public sealed class PvpPhysicalSkillDamageBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVP_PHYSICAL_SKILL_DAMAGE);