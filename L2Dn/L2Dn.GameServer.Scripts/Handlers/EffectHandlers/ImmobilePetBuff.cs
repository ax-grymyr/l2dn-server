using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Immobile Pet Buff effect implementation.
/// </summary>
public sealed class ImmobilePetBuff: AbstractEffect
{
    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.setImmobilized(false);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isSummon() && (effector == effected ||
                (effector.isPlayer() && ((L2Dn.GameServer.Model.Actor.Summon)effected).getOwner() == effector)))
        {
            effected.setImmobilized(true);
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}