using L2Dn.GameServer.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class TransferDamageToSummon(StatSet @params)
    : AbstractStatAddEffect(@params, Stat.TRANSFER_DAMAGE_SUMMON_PERCENT);