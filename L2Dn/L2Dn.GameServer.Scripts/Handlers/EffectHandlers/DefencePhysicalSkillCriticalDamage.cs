using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Equivalent of DefenceMagicCriticalDamage for physical skills.
 * @author Mobius
 */
public class DefencePhysicalSkillCriticalDamage: AbstractStatEffect
{
	public DefencePhysicalSkillCriticalDamage(StatSet @params): base(@params, Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE, Stat.DEFENCE_PHYSICAL_SKILL_CRITICAL_DAMAGE_ADD)
	{
	}
}