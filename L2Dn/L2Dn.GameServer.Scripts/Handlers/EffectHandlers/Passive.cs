using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Passive effect implementation.
/// </summary>
public sealed class Passive: AbstractEffect
{
    public Passive(StatSet @params)
    {
    }

    public override EffectFlags getEffectFlags() => EffectFlags.PASSIVE;

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.isAttackable();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}