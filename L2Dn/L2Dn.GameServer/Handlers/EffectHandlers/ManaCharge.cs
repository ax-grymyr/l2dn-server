using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ManaCharge: AbstractStatAddEffect
{
	public ManaCharge(StatSet @params): base(@params, Stat.MANA_CHARGE)
	{
	}
}