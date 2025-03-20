using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("RearDamage")]
public sealed class RearDamage(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.REAR_DAMAGE_RATE);