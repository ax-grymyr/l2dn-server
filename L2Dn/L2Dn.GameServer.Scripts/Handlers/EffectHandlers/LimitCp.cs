using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("LimitCp")]
public sealed class LimitCp(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.MAX_RECOVERABLE_CP);