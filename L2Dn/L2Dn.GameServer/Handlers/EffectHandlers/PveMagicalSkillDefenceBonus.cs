using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PveMagicalSkillDefenceBonus: AbstractStatPercentEffect
{
	public PveMagicalSkillDefenceBonus(StatSet @params): base(@params, Stat.PVE_MAGICAL_SKILL_DEFENCE)
	{
	}
}