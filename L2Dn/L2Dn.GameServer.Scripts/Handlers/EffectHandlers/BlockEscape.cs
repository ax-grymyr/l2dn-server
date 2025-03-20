using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block escape effect implementation
/// </summary>
[HandlerName("BlockEscape")]
public sealed class BlockEscape: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.CANNOT_ESCAPE;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}