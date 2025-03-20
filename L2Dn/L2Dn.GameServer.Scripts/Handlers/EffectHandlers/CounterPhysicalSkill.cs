using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("CounterPhysicalSkill")]
public sealed class CounterPhysicalSkill(EffectParameterSet parameters)
    : AbstractStatAddEffect(parameters, Stat.VENGEANCE_SKILL_PHYSICAL_DAMAGE);