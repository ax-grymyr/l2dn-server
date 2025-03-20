using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("FocusEnergy")]
public sealed class FocusEnergy(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.MAX_MOMENTUM);