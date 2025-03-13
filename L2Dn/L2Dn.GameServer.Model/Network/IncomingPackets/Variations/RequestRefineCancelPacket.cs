
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.Variations;

public struct RequestRefineCancelPacket: IIncomingPacket<GameSession>
{
    private int _targetItemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _targetItemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item? targetItem = player.getInventory().getItemByObjectId(_targetItemObjId);
        if (targetItem == null)
        {
            player.sendPacket(ExVariationCancelResultPacket.STATIC_PACKET_FAILURE);
            return ValueTask.CompletedTask;
        }

        if (targetItem.getOwnerId() != player.ObjectId)
        {
            Util.handleIllegalPlayerAction(player,
                "Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
                " tryied to augment item that doesn't own.", Config.DEFAULT_PUNISH);

            return ValueTask.CompletedTask;
        }

        // cannot remove augmentation from a not augmented item
        VariationInstance? augmentation = targetItem.getAugmentation();
        if (!targetItem.isAugmented() || augmentation == null)
        {
            player.sendPacket(SystemMessageId.AUGMENTATION_REMOVAL_CAN_ONLY_BE_DONE_ON_AN_AUGMENTED_ITEM);
            player.sendPacket(ExVariationCancelResultPacket.STATIC_PACKET_FAILURE);
            return ValueTask.CompletedTask;
        }

        // get the price
        long price = VariationData.getInstance().getCancelFee(targetItem.getId(), augmentation.getMineralId());
        if (price < 0)
        {
            player.sendPacket(ExVariationCancelResultPacket.STATIC_PACKET_FAILURE);
            return ValueTask.CompletedTask;
        }

        // try to reduce the players adena
        if (!player.reduceAdena("RequestRefineCancel", price, targetItem, true))
        {
            player.sendPacket(ExVariationCancelResultPacket.STATIC_PACKET_FAILURE);
            player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
            return ValueTask.CompletedTask;
        }

        // unequip item
        List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
        if (targetItem.isEquipped())
        {
            foreach (Item itm in player.getInventory().unEquipItemInSlotAndRecord(targetItem.getLocationSlot()))
            {
                itemsToUpdate.Add(new ItemInfo(itm, ItemChangeType.MODIFIED));
            }
        }

        // remove the augmentation
        targetItem.removeAugmentation();

        // send ExVariationCancelResult
        player.sendPacket(ExVariationCancelResultPacket.STATIC_PACKET_SUCCESS);

        // send inventory update
        itemsToUpdate.Add(new ItemInfo(targetItem, ItemChangeType.MODIFIED));

        InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
        player.sendInventoryUpdate(iu);

        return ValueTask.CompletedTask;
    }
}