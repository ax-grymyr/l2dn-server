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

    public override EffectFlags getEffectFlags() => EffectFlags.CANNOT_ESCAPE;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}