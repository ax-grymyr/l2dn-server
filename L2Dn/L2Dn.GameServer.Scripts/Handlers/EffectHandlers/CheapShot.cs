using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class CheapShot: AbstractEffect
{
    public CheapShot(StatSet @params)
    {
    }

    public override long getEffectFlags() => EffectFlag.CHEAPSHOT.getMask();

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}