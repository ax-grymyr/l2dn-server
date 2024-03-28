using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class IgnoreDeath: AbstractEffect
{
	public IgnoreDeath(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.IGNORE_DEATH.getMask();
	}
}