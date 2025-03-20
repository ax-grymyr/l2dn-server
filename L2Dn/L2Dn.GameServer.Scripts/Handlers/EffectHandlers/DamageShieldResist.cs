using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("DamageShieldResist")]
public sealed class DamageShieldResist(EffectParameterSet parameters)
    : AbstractStatAddEffect(parameters, Stat.REFLECT_DAMAGE_PERCENT_DEFENSE);