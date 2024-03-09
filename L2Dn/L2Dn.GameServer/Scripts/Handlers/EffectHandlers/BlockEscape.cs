using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Block escape effect implementation
 * @author UnAfraid
 */
public class BlockEscape: AbstractEffect
{
	public BlockEscape(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.CANNOT_ESCAPE.getMask();
	}
}