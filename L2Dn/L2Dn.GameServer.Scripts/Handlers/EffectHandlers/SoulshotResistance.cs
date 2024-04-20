using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class SoulshotResistance: AbstractStatPercentEffect
{
	public SoulshotResistance(StatSet @params): base(@params, Stat.SOULSHOT_RESISTANCE)
	{
	}
}