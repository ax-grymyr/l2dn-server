using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class CheapShot: AbstractEffect
{
	public CheapShot(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.CHEAPSHOT.getMask();
	}
}