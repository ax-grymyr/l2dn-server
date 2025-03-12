using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Effect that blocks damage and heals to HP/MP.
/// Regeneration or DOT shouldn't be blocked, Vampiric Rage and Balance Life as well.
/// </summary>
public sealed class DamageBlock: AbstractEffect
{
    private readonly bool _blockHp;
    private readonly bool _blockMp;

    public DamageBlock(StatSet @params)
    {
        string type = @params.getString("type", string.Empty);
        _blockHp = type.equalsIgnoreCase("BLOCK_HP");
        _blockMp = type.equalsIgnoreCase("BLOCK_MP");
    }

    public override long getEffectFlags() =>
        _blockHp ? EffectFlag.HP_BLOCK.getMask() :
        _blockMp ? EffectFlag.MP_BLOCK.getMask() : EffectFlag.NONE.getMask();

    public override int GetHashCode() => HashCode.Combine(_blockHp, _blockMp);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._blockHp, x._blockMp));
}