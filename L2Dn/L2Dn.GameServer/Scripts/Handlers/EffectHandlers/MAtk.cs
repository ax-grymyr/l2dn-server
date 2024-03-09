using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class MAtk: AbstractStatEffect
{
	public MAtk(StatSet @params): base(@params, Stat.MAGIC_ATTACK)
	{
	}
}