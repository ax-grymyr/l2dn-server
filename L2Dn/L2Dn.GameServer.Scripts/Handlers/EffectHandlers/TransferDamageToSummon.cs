using L2Dn.GameServer.Handlers;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("TransferDamageToSummon")]
public sealed class TransferDamageToSummon(EffectParameterSet parameters)
    : AbstractStatAddEffect(parameters, Stat.TRANSFER_DAMAGE_SUMMON_PERCENT);