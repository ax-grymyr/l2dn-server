using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class SpiritshotResistance: AbstractStatPercentEffect
{
	public SpiritshotResistance(StatSet @params): base(@params, Stat.SPIRITSHOT_RESISTANCE)
	{
	}
}