using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class AgathionSlot: AbstractStatAddEffect
{
	public AgathionSlot(StatSet @params): base(@params, Stat.AGATHION_SLOTS)
	{
	}
}