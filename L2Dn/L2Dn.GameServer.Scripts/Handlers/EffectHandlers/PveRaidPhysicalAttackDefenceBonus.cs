using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("PveRaidPhysicalAttackDefenceBonus")]
public sealed class PveRaidPhysicalAttackDefenceBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVE_RAID_PHYSICAL_ATTACK_DEFENCE);