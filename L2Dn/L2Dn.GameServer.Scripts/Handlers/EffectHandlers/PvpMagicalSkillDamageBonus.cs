using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PvpMagicalSkillDamageBonus")]
public sealed class PvpMagicalSkillDamageBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVP_MAGICAL_SKILL_DAMAGE);