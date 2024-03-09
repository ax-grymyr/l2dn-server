using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class CounterPhysicalSkill: AbstractStatAddEffect
{
	public CounterPhysicalSkill(StatSet @params): base(@params, Stat.VENGEANCE_SKILL_PHYSICAL_DAMAGE)
	{
	}
}