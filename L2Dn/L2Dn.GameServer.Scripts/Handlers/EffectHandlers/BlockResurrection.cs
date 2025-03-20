using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block Resurrection effect implementation.
/// </summary>
public sealed class BlockResurrection: AbstractEffect
{
    public BlockResurrection(StatSet @params)
    {
    }

    public override EffectFlags EffectFlags => EffectFlags.BLOCK_RESURRECTION;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}