using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Immobile Buff effect implementation.
/// </summary>
[HandlerStringKey("BlockMove")]
public sealed class BlockMove: AbstractEffect
{
    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.setImmobilized(true);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.setImmobilized(false);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}