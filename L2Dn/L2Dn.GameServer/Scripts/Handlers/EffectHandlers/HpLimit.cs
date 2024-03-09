using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author dontknowdontcare
 */
public class HpLimit: AbstractStatEffect
{
	public HpLimit(StatSet @params): base(@params, Stat.HP_LIMIT)
	{
	}
}