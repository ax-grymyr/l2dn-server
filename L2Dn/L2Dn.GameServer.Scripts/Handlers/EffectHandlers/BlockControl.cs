using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// An effect that blocks the player (NPC?) control. It prevents moving, casting, social actions, etc.
/// </summary>
public sealed class BlockControl: AbstractEffect
{
    public BlockControl(StatSet @params)
    {
    }

    public override long getEffectFlags() => EffectFlag.BLOCK_CONTROL.getMask();

    public override EffectType getEffectType() => EffectType.BLOCK_CONTROL;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}