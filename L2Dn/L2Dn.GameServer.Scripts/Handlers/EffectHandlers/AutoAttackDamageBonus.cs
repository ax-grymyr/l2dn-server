using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("AutoAttackDamageBonus")]
public sealed class AutoAttackDamageBonus(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.AUTO_ATTACK_DAMAGE_BONUS);