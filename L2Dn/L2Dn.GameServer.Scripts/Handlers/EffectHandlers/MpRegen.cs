using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("MpRegen")]
public sealed class MpRegen(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.REGENERATE_MP_RATE);