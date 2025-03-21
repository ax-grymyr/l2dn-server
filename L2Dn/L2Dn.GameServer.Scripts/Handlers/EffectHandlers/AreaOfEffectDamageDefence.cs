using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("AreaOfEffectDamageDefence")]
public sealed class AreaOfEffectDamageDefence(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.AREA_OF_EFFECT_DAMAGE_DEFENCE);