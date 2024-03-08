using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DefenceCriticalDamage: AbstractStatEffect
{
	public DefenceCriticalDamage(StatSet @params): base(@params, Stat.DEFENCE_CRITICAL_DAMAGE, Stat.DEFENCE_CRITICAL_DAMAGE_ADD)
	{
	}
}