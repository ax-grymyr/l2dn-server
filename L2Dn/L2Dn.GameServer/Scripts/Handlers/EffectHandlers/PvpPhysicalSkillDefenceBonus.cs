using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PvpPhysicalSkillDefenceBonus: AbstractStatPercentEffect
{
	public PvpPhysicalSkillDefenceBonus(StatSet @params): base(@params, Stat.PVP_PHYSICAL_SKILL_DEFENCE)
	{
	}
}