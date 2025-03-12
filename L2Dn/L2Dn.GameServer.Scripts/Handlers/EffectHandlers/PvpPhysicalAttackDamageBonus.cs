using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PvpPhysicalAttackDamageBonus(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.PVP_PHYSICAL_ATTACK_DAMAGE);