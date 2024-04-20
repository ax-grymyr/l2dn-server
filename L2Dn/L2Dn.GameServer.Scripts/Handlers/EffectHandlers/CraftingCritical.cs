using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Nik
 */
public class CraftingCritical: AbstractStatAddEffect
{
	public CraftingCritical(StatSet @params): base(@params, Stat.CRAFTING_CRITICAL)
	{
	}
}