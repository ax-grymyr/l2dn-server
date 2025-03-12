using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PvpPhysicalSkillDamageBonus(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.PVP_PHYSICAL_SKILL_DAMAGE);