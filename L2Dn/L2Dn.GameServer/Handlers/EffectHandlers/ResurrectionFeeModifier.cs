using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class ResurrectionFeeModifier: AbstractStatEffect
{
	public ResurrectionFeeModifier(StatSet @params): base(@params, Stat.RESURRECTION_FEE_MODIFIER)
	{
	}
}