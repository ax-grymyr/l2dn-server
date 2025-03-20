using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("AgathionSlot")]
public sealed class AgathionSlot(EffectParameterSet parameters): AbstractStatAddEffect(parameters, Stat.AGATHION_SLOTS);