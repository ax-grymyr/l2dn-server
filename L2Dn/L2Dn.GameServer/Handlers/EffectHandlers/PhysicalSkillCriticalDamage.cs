using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalSkillCriticalDamage: AbstractStatEffect
{
	public PhysicalSkillCriticalDamage(StatSet @params)
		: base(@params, Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE, Stat.PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD)
	{
	}
}