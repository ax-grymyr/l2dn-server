using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalSkillPower: AbstractStatEffect
{
	public PhysicalSkillPower(StatSet @params)
		: base(@params, Stat.PHYSICAL_SKILL_POWER, Stat.SKILL_POWER_ADD)
	{
	}
}