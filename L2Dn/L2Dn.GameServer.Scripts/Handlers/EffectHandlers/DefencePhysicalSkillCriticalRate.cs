using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class DefencePhysicalSkillCriticalRate: AbstractStatEffect
{
	public DefencePhysicalSkillCriticalRate(StatSet @params): base(@params, Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE, Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_RATE_ADD)
	{
	}
}