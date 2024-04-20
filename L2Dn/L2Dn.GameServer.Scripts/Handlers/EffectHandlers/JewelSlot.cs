using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class JewelSlot: AbstractStatAddEffect
{
	public JewelSlot(StatSet @params): base(@params, Stat.BROOCH_JEWELS)
	{
	}
}