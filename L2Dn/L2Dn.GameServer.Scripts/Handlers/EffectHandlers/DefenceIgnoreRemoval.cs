using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class DefenceIgnoreRemoval(StatSet @params)
    : AbstractStatEffect(@params, Stat.DEFENCE_IGNORE_REMOVAL, Stat.DEFENCE_IGNORE_REMOVAL_ADD);