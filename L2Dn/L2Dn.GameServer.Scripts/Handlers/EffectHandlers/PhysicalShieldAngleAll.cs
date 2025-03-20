using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class PhysicalShieldAngleAll: AbstractEffect
{
    public PhysicalShieldAngleAll(StatSet @params)
    {
    }

    public override EffectFlags getEffectFlags() => EffectFlags.PHYSICAL_SHIELD_ANGLE_ALL;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}