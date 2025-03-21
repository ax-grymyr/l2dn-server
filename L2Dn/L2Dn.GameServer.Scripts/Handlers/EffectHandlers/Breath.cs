using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("Breath")]
public sealed class Breath(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.BREATH);