using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author quangnguyen
 */
public class CraftRate: AbstractStatAddEffect
{
	public CraftRate(StatSet @params): base(@params, Stat.CRAFT_RATE)
	{
	}
}