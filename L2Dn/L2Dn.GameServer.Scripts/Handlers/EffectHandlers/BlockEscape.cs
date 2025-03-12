using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block escape effect implementation
/// </summary>
public sealed class BlockEscape: AbstractEffect
{
    public BlockEscape(StatSet @params)
    {
    }

    public override long getEffectFlags() => EffectFlag.CANNOT_ESCAPE.getMask();

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}