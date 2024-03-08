using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class PhysicalEvasion: AbstractConditionalHpEffect
{
	public PhysicalEvasion(StatSet @params): base(@params, Stat.EVASION_RATE)
	{
	}
}