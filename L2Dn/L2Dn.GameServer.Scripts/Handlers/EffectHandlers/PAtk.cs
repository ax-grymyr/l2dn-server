using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("PAtk")]
public sealed class PAtk(EffectParameterSet parameters): AbstractConditionalHpEffect(parameters, Stat.PHYSICAL_ATTACK);