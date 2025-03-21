using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Enable Cloak effect implementation.
/// </summary>
[HandlerStringKey("EnableCloak")]
public sealed class EnableCloak: AbstractEffect
{
    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effector != null && effected != null && effected.isPlayer();
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        effected.getActingPlayer()?.getStat().setCloakSlotStatus(true);
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.getActingPlayer()?.getStat().setCloakSlotStatus(false);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}