using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("DefenceIgnoreRemoval")]
public sealed class DefenceIgnoreRemoval(EffectParameterSet parameters)
    : AbstractStatEffect(parameters, Stat.DEFENCE_IGNORE_REMOVAL, Stat.DEFENCE_IGNORE_REMOVAL_ADD);