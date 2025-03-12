using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PvePhysicalAttackDefenceBonus(StatSet @params)
    : AbstractStatPercentEffect(@params, Stat.PVE_PHYSICAL_ATTACK_DEFENCE);