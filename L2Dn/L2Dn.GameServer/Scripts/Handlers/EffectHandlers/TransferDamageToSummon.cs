using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class TransferDamageToSummon: AbstractStatAddEffect
{
	public TransferDamageToSummon(StatSet @params): base(@params, Stat.TRANSFER_DAMAGE_SUMMON_PERCENT)
	{
	}
}