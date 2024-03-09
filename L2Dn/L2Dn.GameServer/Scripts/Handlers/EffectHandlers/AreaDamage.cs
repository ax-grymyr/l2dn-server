using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class AreaDamage: AbstractStatAddEffect
{
	public AreaDamage(StatSet @params): base(@params, Stat.DAMAGE_ZONE_VULN)
	{
	}
}