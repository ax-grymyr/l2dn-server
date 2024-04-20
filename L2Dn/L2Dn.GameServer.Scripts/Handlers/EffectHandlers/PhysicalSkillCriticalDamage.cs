using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

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