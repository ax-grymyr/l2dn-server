using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("NewHennaSlot")]
public sealed class NewHennaSlot(EffectParameterSet parameters):
    AbstractStatAddEffect(parameters, Stat.HENNA_SLOTS_AVAILABLE);