using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class ProtectDeathPenalty: AbstractEffect
{
	public ProtectDeathPenalty(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.PROTECT_DEATH_PENALTY.getMask();
	}
}