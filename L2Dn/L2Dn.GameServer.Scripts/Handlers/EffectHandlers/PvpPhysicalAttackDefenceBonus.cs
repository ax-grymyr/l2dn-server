using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PvpPhysicalAttackDefenceBonus")]
public sealed class PvpPhysicalAttackDefenceBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVP_PHYSICAL_ATTACK_DEFENCE);