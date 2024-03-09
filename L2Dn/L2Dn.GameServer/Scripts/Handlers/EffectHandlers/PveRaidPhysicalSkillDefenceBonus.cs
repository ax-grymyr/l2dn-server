using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PveRaidPhysicalSkillDefenceBonus: AbstractStatPercentEffect
{
	public PveRaidPhysicalSkillDefenceBonus(StatSet @params): base(@params, Stat.PVE_RAID_PHYSICAL_SKILL_DEFENCE)
	{
	}
}