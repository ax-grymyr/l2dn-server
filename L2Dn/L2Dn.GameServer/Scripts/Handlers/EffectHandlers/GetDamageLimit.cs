using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class GetDamageLimit: AbstractStatAddEffect
{
	public GetDamageLimit(StatSet @params): base(@params, Stat.DAMAGE_LIMIT)
	{
	}
}