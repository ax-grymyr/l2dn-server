using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PveRaidMagicalSkillDefenceBonus(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.PVE_RAID_MAGICAL_SKILL_DEFENCE);