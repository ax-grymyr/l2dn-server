using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("PveRaidPhysicalSkillDamageBonus")]
public sealed class PveRaidPhysicalSkillDamageBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVE_RAID_PHYSICAL_SKILL_DAMAGE);