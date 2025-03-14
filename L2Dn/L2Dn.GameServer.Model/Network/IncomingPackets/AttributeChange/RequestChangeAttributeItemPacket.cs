using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.AttributeChange;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.AttributeChange;

public struct RequestChangeAttributeItemPacket: IIncomingPacket<GameSession>
{
    private int _consumeItemId;
    private int _itemObjId;
    private AttributeType _newElementId;

    public void ReadContent(PacketBitReader reader)
    {
        _consumeItemId = reader.ReadInt32();
        _itemObjId = reader.ReadInt32();
        _newElementId = (AttributeType)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        PlayerInventory inventory = player.getInventory();
        Item? item = inventory.getItemByObjectId(_itemObjId);
        if (item is null)
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // attempting to destroy item
        if (player.getInventory().destroyItemByItemId("ChangeAttribute", _consumeItemId, 1, player, item) == null)
        {
            player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
            player.sendPacket(ExChangeAttributeFailPacket.STATIC);

            Util.handleIllegalPlayerAction(player,
                player + " tried to change attribute without an attribute change crystal.", Config.General.DEFAULT_PUNISH);

            return ValueTask.CompletedTask;
        }

        // get values
        AttributeType oldElementId = item.getAttackAttributeType();
        int elementValue = item.getAttackAttribute()?.getValue() ?? 0;
        item.clearAllAttributes();
        item.setAttribute(new AttributeHolder(_newElementId, elementValue), true);

        // send packets
        SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.S1_S_S2_ATTRIBUTE_HAS_SUCCESSFULLY_CHANGED_TO_S3_ATTRIBUTE);
        msg.Params.addItemName(item);
        msg.Params.addAttribute(oldElementId);
        msg.Params.addAttribute(_newElementId);
        player.sendPacket(msg);

        List<ItemInfo> itemsToUpdate = [new(item, ItemChangeType.MODIFIED)];
        foreach (Item i in player.getInventory().getAllItemsByItemId(_consumeItemId))
            itemsToUpdate.Add(new ItemInfo(i));

        InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
        player.sendPacket(iu);
        player.broadcastUserInfo();
        player.sendPacket(ExChangeAttributeOkPacket.STATIC);

        return ValueTask.CompletedTask;
    }
}