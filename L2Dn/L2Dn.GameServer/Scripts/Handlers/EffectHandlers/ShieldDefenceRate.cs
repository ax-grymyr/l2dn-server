using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ShieldDefenceRate: AbstractStatEffect
{
	public ShieldDefenceRate(StatSet @params): base(@params, Stat.SHIELD_DEFENCE_RATE)
	{
	}
}