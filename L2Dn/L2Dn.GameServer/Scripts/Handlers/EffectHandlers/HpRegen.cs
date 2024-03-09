using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class HpRegen: AbstractConditionalHpEffect
{
	public HpRegen(StatSet @params): base(@params, Stat.REGENERATE_HP_RATE)
	{
	}
}