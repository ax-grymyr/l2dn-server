using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Noblesse Blessing effect implementation.
/// </summary>
[HandlerName("NoblesseBless")]
public sealed class NoblesseBless: AbstractEffect
{
    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effector != null && effected != null && effected.isPlayable();
    }

    public override EffectFlags EffectFlags => EffectFlags.NOBLESS_BLESSING;

    public override EffectTypes EffectTypes => EffectTypes.NOBLESSE_BLESSING;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}