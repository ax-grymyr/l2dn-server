using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Effect that blocks all incoming debuffs.
 * @author Nik
 */
public class DebuffBlock: AbstractEffect
{
	public DebuffBlock(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.DEBUFF_BLOCK.getMask();
	}
}