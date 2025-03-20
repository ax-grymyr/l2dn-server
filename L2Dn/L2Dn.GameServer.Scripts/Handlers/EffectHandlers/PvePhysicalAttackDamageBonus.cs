using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("PvePhysicalAttackDamageBonus")]
public sealed class PvePhysicalAttackDamageBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVE_PHYSICAL_ATTACK_DAMAGE);