using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalSkillPower(StatSet @params)
    : AbstractStatEffect(@params, Stat.PHYSICAL_SKILL_POWER, Stat.SKILL_POWER_ADD);