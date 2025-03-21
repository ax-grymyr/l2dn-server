using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("MagicAccuracy")]
public sealed class MagicAccuracy(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.ACCURACY_MAGIC);