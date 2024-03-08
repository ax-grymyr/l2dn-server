using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Effect that blocks all incoming debuffs.
 * @author Nik
 */
public class BuffBlock: AbstractEffect
{
	public BuffBlock(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.BUFF_BLOCK.getMask();
	}
}