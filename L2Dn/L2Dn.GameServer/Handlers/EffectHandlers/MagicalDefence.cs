using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class MagicalDefence: AbstractStatEffect
{
	public MagicalDefence(StatSet @params): base(@params, Stat.MAGICAL_DEFENCE)
	{
	}
}