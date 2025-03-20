using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Noblesse Blessing effect implementation.
/// </summary>
public sealed class NoblesseBless: AbstractEffect
{
    public NoblesseBless(StatSet @params)
    {
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effector != null && effected != null && effected.isPlayable();
    }

    public override EffectFlags getEffectFlags() => EffectFlags.NOBLESS_BLESSING;

    public override EffectTypes EffectType => EffectTypes.NOBLESSE_BLESSING;

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}