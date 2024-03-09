using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PvpMagicalSkillDamageBonus: AbstractStatPercentEffect
{
	public PvpMagicalSkillDamageBonus(StatSet @params): base(@params, Stat.PVP_MAGICAL_SKILL_DAMAGE)
	{
	}
}