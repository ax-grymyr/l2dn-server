using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class SkillMasteryRate: AbstractStatPercentEffect
{
	public SkillMasteryRate(StatSet @params): base(@params, Stat.SKILL_MASTERY_RATE)
	{
	}
}