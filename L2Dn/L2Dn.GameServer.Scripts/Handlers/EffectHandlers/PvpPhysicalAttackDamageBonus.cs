using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PvpPhysicalAttackDamageBonus")]
public sealed class PvpPhysicalAttackDamageBonus(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.PVP_PHYSICAL_ATTACK_DAMAGE);