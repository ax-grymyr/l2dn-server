using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Effect that blocks all incoming debuffs.
/// </summary>
[AbstractEffectName("DebuffBlock")]
public sealed class DebuffBlock: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.DEBUFF_BLOCK;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}