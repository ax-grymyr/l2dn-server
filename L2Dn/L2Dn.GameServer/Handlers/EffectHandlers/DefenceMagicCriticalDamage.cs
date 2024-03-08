using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DefenceMagicCriticalDamage: AbstractStatEffect
{
	public DefenceMagicCriticalDamage(StatSet @params): base(@params, Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE, Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD)
	{
	}
}