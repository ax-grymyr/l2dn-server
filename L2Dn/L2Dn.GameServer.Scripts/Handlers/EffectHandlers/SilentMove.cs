using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Silent Move effect implementation.
/// </summary>
public sealed class SilentMove: AbstractEffect
{
    public SilentMove(StatSet @params)
    {
    }

    public override long getEffectFlags() => EffectFlag.SILENT_MOVE.getMask();

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}