using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Block Resurrection effect implementation.
 * @author UnAfraid
 */
public class BlockResurrection: AbstractEffect
{
	public BlockResurrection(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.BLOCK_RESURRECTION.getMask();
	}
}