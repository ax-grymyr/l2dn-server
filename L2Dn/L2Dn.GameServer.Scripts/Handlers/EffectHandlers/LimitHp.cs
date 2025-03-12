using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class LimitHp(StatSet @params): AbstractStatEffect(@params, Stat.MAX_RECOVERABLE_HP);