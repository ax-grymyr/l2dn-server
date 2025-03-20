using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("MpShield")]
public sealed class MpShield(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.MANA_SHIELD_PERCENT);