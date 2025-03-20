using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Lucky effect implementation.
/// </summary>
[HandlerName("Lucky")]
public sealed class Lucky: AbstractEffect
{
    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effector != null && effected != null && effected.isPlayer();
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}