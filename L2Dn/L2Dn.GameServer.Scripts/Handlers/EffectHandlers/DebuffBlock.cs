using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Effect that blocks all incoming debuffs.
/// </summary>
public sealed class DebuffBlock: AbstractEffect
{
    public DebuffBlock(StatSet @params)
    {
    }

    public override long getEffectFlags() => EffectFlag.DEBUFF_BLOCK.getMask();

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}