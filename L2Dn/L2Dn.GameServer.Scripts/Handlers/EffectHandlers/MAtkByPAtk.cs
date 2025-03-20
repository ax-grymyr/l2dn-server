using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("MAtkByPAtk")]
public sealed class MAtkByPAtk(EffectParameterSet parameters):
    AbstractStatPercentEffect(parameters, Stat.MAGIC_ATTACK_BY_PHYSICAL_ATTACK);