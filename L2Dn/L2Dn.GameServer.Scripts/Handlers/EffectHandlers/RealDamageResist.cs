using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("RealDamageResist")]
public sealed class RealDamageResist(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.REAL_DAMAGE_RESIST);