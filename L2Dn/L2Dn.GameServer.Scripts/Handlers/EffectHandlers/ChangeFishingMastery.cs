using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Change Fishing Mastery dummy effect implementation.
/// </summary>
public sealed class ChangeFishingMastery: AbstractEffect
{
    public ChangeFishingMastery(StatSet @params)
    {
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}