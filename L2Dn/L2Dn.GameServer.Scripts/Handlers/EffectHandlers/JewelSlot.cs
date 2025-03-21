using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerStringKey("JewelSlot")]
public sealed class JewelSlot(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.BROOCH_JEWELS);