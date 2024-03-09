using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class MagicCriticalDamage: AbstractStatEffect
{
	public MagicCriticalDamage(StatSet @params)
		: base(@params, Stat.MAGIC_CRITICAL_DAMAGE, Stat.MAGIC_CRITICAL_DAMAGE_ADD)
	{
	}
}