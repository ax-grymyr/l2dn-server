using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class CriticalDamage: AbstractStatEffect
{
	public CriticalDamage(StatSet @params): base(@params, Stat.CRITICAL_DAMAGE, Stat.CRITICAL_DAMAGE_ADD)
	{
	}
}