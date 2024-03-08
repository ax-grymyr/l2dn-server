using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalDefence: AbstractConditionalHpEffect
{
	public PhysicalDefence(StatSet @params): base(@params, Stat.PHYSICAL_DEFENCE)
	{
	}
}