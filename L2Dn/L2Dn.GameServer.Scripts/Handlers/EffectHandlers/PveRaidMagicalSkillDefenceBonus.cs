using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PveRaidMagicalSkillDefenceBonus")]
public sealed class PveRaidMagicalSkillDefenceBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVE_RAID_MAGICAL_SKILL_DEFENCE);