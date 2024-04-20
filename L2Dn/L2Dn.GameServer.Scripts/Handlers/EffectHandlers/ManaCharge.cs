using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ManaCharge: AbstractStatAddEffect
{
	public ManaCharge(StatSet @params): base(@params, Stat.MANA_CHARGE)
	{
	}
}