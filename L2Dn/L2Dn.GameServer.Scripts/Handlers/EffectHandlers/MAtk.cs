using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("MAtk")]
public sealed class MAtk(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.MAGIC_ATTACK);