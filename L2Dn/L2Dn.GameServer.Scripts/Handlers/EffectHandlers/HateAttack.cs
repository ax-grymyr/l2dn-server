using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("HateAttack")]
public sealed class HateAttack(EffectParameterSet parameters): AbstractStatPercentEffect(parameters, Stat.HATE_ATTACK);