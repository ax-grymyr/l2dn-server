using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Silent Move effect implementation.
/// </summary>
[HandlerName("SilentMove")]
public sealed class SilentMove: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.SILENT_MOVE;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}