using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class MagicalSkillPower: AbstractStatEffect
{
	public MagicalSkillPower(StatSet @params): base(@params, Stat.MAGICAL_SKILL_POWER, Stat.SKILL_POWER_ADD)
	{
	}
}