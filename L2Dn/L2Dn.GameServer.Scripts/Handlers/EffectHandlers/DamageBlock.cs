using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Effect that blocks damage and heals to HP/MP.<br>
 * Regeneration or DOT shouldn't be blocked, Vampiric Rage and Balance Life as well.
 * @author Nik
 */
public class DamageBlock: AbstractEffect
{
	private readonly bool _blockHp;
	private readonly bool _blockMp;
	
	public DamageBlock(StatSet @params)
	{
		string type = @params.getString("type", null);
		_blockHp = type.equalsIgnoreCase("BLOCK_HP");
		_blockMp = type.equalsIgnoreCase("BLOCK_MP");
	}
	
	public override long getEffectFlags()
	{
		return _blockHp ? EffectFlag.HP_BLOCK.getMask() : (_blockMp ? EffectFlag.MP_BLOCK.getMask() : EffectFlag.NONE.getMask());
	}
}