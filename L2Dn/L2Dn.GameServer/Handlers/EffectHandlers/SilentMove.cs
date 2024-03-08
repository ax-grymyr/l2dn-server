using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Silent Move effect implementation.
 */
public class SilentMove: AbstractEffect
{
	public SilentMove(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.SILENT_MOVE.getMask();
	}
}