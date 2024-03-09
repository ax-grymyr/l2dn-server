using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author NasSeKa
 */
public class PhysicalSkillCriticalRate: AbstractStatPercentEffect
{
	public PhysicalSkillCriticalRate(StatSet @params): base(@params, Stat.CRITICAL_RATE_SKILL)
	{
	}
}