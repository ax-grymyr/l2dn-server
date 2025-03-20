using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("HitNumber")]
public sealed class HitNumber(EffectParameterSet parameters): AbstractStatEffect(parameters, Stat.ATTACK_COUNT_MAX);