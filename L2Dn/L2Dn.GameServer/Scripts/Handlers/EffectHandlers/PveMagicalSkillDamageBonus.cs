using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PveMagicalSkillDamageBonus: AbstractStatPercentEffect
{
	public PveMagicalSkillDamageBonus(StatSet @params): base(@params, Stat.PVE_MAGICAL_SKILL_DAMAGE)
	{
	}
}