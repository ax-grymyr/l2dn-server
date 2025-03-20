using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("ManaCharge")]
public sealed class ManaCharge(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.MANA_CHARGE);