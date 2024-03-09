using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class HateAttack: AbstractStatPercentEffect
{
	public HateAttack(StatSet @params): base(@params, Stat.HATE_ATTACK)
	{
	}
}