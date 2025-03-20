using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class IgnoreDeath: AbstractEffect
{
    public IgnoreDeath(StatSet @params)
    {
    }

    public override EffectFlags EffectFlags => EffectFlags.IGNORE_DEATH;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}