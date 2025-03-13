using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Ensoul;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.EnsoulPackets;

public struct RequestTryEnSoulExtractionPacket: IIncomingPacket<GameSession>
{
    private int _itemObjectId;
    private int _type;
    private int _position;

    public void ReadContent(PacketBitReader reader)
    {
        _itemObjectId = reader.ReadInt32();
        _type = reader.ReadByte();
        _position = reader.ReadByte() - 1;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Item? item = player.getInventory().getItemByObjectId(_itemObjectId);
        if (item == null)
            return ValueTask.CompletedTask;

        EnsoulOption? option = null;
        if (_type == 1)
        {
            option = item.getSpecialAbility(_position);
            // If position is invalid, check the other one.
            if (option == null && _position == 0)
            {
                option = item.getSpecialAbility(1);
                if (option != null)
                {
                    _position = 1;
                }
            }
        }

        if (_type == 2)
        {
            option = item.getAdditionalSpecialAbility(_position);
        }

        if (option == null)
            return ValueTask.CompletedTask;

        int runeId = EnsoulData.getInstance().getStone(_type, option.getId());
        ICollection<ItemHolder> removalFee = EnsoulData.getInstance().getRemovalFee(item.getTemplate().getCrystalType()); // it was runeId
        if (removalFee.Count == 0)
            return ValueTask.CompletedTask;

        // Check if player has required items.
        foreach (ItemHolder itemHolder in removalFee)
        {
            if (player.getInventory().getInventoryItemCount(itemHolder.getId(), -1) < itemHolder.getCount())
            {
                player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT);
                player.sendPacket(new ExEnSoulExtractionResultPacket(false, item));
                return ValueTask.CompletedTask;
            }
        }

        // Take required items.
        foreach (ItemHolder itemHolder in removalFee)
        {
            player.destroyItemByItemId("Rune Extract", itemHolder.getId(), itemHolder.getCount(), player, true);
        }

        // Remove equipped rune.
        item.removeSpecialAbility(_position, _type);

        List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
        itemsToUpdate.Add(new ItemInfo(item, ItemChangeType.MODIFIED));

        // Add rune in player inventory.
        if (runeId > 0)
        {
            Item? addItem = player.addItem("Rune Extract", runeId, 1, player, true);
            if (addItem == null)
            {
                player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: atomic inventory update
                return ValueTask.CompletedTask;
            }

            itemsToUpdate.Add(new ItemInfo(addItem));
        }

        InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
        player.sendInventoryUpdate(iu);
        player.sendItemList();

        player.sendPacket(new ExEnSoulExtractionResultPacket(true, item));

        return ValueTask.CompletedTask;
    }
}