using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PveRaidMagicalSkillDefenceBonus: AbstractStatPercentEffect
{
	public PveRaidMagicalSkillDefenceBonus(StatSet @params): base(@params, Stat.PVE_RAID_MAGICAL_SKILL_DEFENCE)
	{
	}
}