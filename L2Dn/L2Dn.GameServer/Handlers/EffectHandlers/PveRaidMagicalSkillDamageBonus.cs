using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class PveRaidMagicalSkillDamageBonus: AbstractStatPercentEffect
{
	public PveRaidMagicalSkillDamageBonus(StatSet @params): base(@params, Stat.PVE_RAID_MAGICAL_SKILL_DAMAGE)
	{
	}
}