using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("ShieldDefence")]
public sealed class ShieldDefence(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.SHIELD_DEFENCE);