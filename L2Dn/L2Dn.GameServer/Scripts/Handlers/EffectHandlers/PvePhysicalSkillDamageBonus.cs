using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PvePhysicalSkillDamageBonus: AbstractStatPercentEffect
{
	public PvePhysicalSkillDamageBonus(StatSet @params): base(@params, Stat.PVE_PHYSICAL_SKILL_DAMAGE)
	{
	}
}