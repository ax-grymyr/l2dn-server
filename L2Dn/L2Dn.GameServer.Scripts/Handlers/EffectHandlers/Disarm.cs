using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Disarm effect implementation.
/// </summary>
public sealed class Disarm: AbstractEffect
{
    private static readonly Map<int, int> _disarmedPlayers = new();

    public Disarm(StatSet @params)
    {
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return effected.isPlayer();
    }

    public override EffectFlags EffectFlags => EffectFlags.DISARMED;

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player is null)
            return;

        Item? itemToDisarm = player.getInventory().getPaperdollItem(Inventory.PAPERDOLL_RHAND);
        if (itemToDisarm == null)
            return;

        long slot = player.getInventory().getSlotFromItem(itemToDisarm);
        player.getInventory().unEquipItemInBodySlot(slot);

        InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(itemToDisarm, ItemChangeType.MODIFIED));
        player.sendInventoryUpdate(iu);
        player.broadcastUserInfo();

        _disarmedPlayers.put(player.ObjectId, itemToDisarm.ObjectId);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        int itemObjectId = _disarmedPlayers.remove(effected.ObjectId);
        if (itemObjectId == 0)
            return;

        Item? item = player.getInventory().getItemByObjectId(itemObjectId);
        if (item == null)
            return;

        player.getInventory().equipItem(item);
        InventoryUpdatePacket iu = new InventoryUpdatePacket(new ItemInfo(item, ItemChangeType.MODIFIED));
        player.sendInventoryUpdate(iu);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}