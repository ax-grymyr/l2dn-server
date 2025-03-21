using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Sweeper effect implementation.
/// </summary>
[HandlerStringKey("Sweeper")]
public sealed class Sweeper: AbstractEffect
{
    public Sweeper(EffectParameterSet parameters)
    {
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (!effector.isPlayer() || player == null || !effected.isAttackable())
            return;

        Attackable monster = (Attackable)effected;
        if (!monster.checkSpoilOwner(player, false))
        {
            return;
        }

        if (!player.getInventory().checkInventorySlotsAndWeight(monster.getSpoilLootItems(), false, false))
        {
            return;
        }

        ICollection<ItemHolder>? items = monster.takeSweep();
        if (items != null)
        {
            foreach (ItemHolder sweepedItem in items)
            {
                Party? party = player.getParty();
                if (party != null)
                    party.distributeItem(player, sweepedItem, true, monster);
                else
                    player.addItem("Sweeper", sweepedItem, effected, true);
            }
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}