using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class SkillPowerAdd: AbstractStatAddEffect
{
	public SkillPowerAdd(StatSet @params): base(@params, Stat.SKILL_POWER_ADD)
	{
	}
}