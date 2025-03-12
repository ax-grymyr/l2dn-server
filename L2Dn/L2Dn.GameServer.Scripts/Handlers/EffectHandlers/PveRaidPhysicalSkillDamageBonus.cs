using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PveRaidPhysicalSkillDamageBonus(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.PVE_RAID_PHYSICAL_SKILL_DAMAGE);