using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("AreaOfEffectDamageModify")]
public sealed class AreaOfEffectDamageModify(EffectParameterSet parameters)
    : AbstractStatPercentEffect(parameters, Stat.AREA_OF_EFFECT_DAMAGE_MODIFY);