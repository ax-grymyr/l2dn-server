using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

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