using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Change Fishing Mastery dummy effect implementation.
/// </summary>
[AbstractEffectName("ChangeFishingMastery")]
public sealed class ChangeFishingMastery: AbstractEffect
{
    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}