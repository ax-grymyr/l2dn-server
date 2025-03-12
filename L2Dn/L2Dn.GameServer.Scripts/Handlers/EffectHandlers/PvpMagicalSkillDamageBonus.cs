using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PvpMagicalSkillDamageBonus(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.PVP_MAGICAL_SKILL_DAMAGE);