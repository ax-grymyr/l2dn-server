using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * An effect that blocks the player (NPC?) control.<br>
 * It prevents moving, casting, social actions, etc.
 * @author Nik
 */
public class BlockControl: AbstractEffect
{
	public BlockControl(StatSet @params)
	{
	}
	
	public override long getEffectFlags()
	{
		return EffectFlag.BLOCK_CONTROL.getMask();
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.BLOCK_CONTROL;
	}
}