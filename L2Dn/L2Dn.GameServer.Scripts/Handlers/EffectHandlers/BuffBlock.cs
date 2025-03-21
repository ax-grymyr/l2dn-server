using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Effect that blocks all incoming debuffs.
/// </summary>
[HandlerStringKey("BuffBlock")]
public sealed class BuffBlock: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.BUFF_BLOCK;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}