using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class JewelSlot: AbstractStatAddEffect
{
	public JewelSlot(StatSet @params): base(@params, Stat.BROOCH_JEWELS)
	{
	}
}