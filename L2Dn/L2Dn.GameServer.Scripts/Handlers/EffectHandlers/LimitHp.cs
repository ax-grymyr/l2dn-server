using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("LimitHp")]
public sealed class LimitHp(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.MAX_RECOVERABLE_HP);