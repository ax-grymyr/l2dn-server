using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class MagicalEvasion: AbstractStatEffect
{
	public MagicalEvasion(StatSet @params): base(@params, Stat.MAGIC_EVASION_RATE)
	{
	}
}