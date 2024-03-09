using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class Breath: AbstractStatEffect
{
	public Breath(StatSet @params): base(@params, Stat.BREATH)
	{
	}
}