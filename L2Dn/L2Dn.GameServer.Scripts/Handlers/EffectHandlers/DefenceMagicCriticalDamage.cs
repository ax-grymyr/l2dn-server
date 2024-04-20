using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class DefenceMagicCriticalDamage: AbstractStatEffect
{
	public DefenceMagicCriticalDamage(StatSet @params): base(@params, Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE, Stat.DEFENCE_MAGIC_CRITICAL_DAMAGE_ADD)
	{
	}
}