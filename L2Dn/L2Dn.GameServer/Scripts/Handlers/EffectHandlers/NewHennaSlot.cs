using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Serenitty
 */
public class NewHennaSlot: AbstractStatAddEffect
{
	public NewHennaSlot(StatSet @params): base(@params, Stat.HENNA_SLOTS_AVAILABLE)
	{
	}
}