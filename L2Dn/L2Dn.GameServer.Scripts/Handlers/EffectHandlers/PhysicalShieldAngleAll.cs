using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Effects;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("PhysicalShieldAngleAll")]
public sealed class PhysicalShieldAngleAll: AbstractEffect
{
    public override EffectFlags EffectFlags => EffectFlags.PHYSICAL_SHIELD_ANGLE_ALL;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}