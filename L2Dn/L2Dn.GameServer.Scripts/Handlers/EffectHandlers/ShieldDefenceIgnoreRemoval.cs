using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("ShieldDefenceIgnoreRemoval")]
public sealed class ShieldDefenceIgnoreRemoval(EffectParameterSet parameters): AbstractStatEffect(parameters,
    Stat.SHIELD_DEFENCE_IGNORE_REMOVAL, Stat.SHIELD_DEFENCE_IGNORE_REMOVAL_ADD);