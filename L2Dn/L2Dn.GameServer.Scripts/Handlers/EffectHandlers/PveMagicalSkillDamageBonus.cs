using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PveMagicalSkillDamageBonus")]
public sealed class PveMagicalSkillDamageBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVE_MAGICAL_SKILL_DAMAGE);