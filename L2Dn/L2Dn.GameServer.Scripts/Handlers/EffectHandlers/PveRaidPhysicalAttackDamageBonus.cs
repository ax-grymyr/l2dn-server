using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PveRaidPhysicalAttackDamageBonus")]
public sealed class PveRaidPhysicalAttackDamageBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVE_RAID_PHYSICAL_ATTACK_DAMAGE);