using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.CompoundEnchanting;

public struct RequestNewEnchantPushTwoPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.isInStoreMode())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_IN_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
            player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (player.isProcessingTransaction() || player.isProcessingRequest())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP);
            player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        CompoundRequest request = player.getRequest<CompoundRequest>();
        if (request == null || request.isProcessing())
        {
            player.sendPacket(ExEnchantTwoFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Make sure player owns this item.
        request.setItemTwo(_objectId);
        Item itemOne = request.getItemOne();
        Item itemTwo = request.getItemTwo();
        if (itemOne == null || itemTwo == null)
        {
            player.sendPacket(ExEnchantTwoFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Lets prevent using same item twice. Also stackable item check.
        if (itemOne.ObjectId == itemTwo.ObjectId && (!itemOne.isStackable() ||
            player.getInventory().getInventoryItemCount(itemOne.getTemplate().getId(), -1) < 2))
        {
            player.sendPacket(ExEnchantTwoFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        CombinationItem? combinationItem = CombinationItemsData.getInstance().getItemsBySlots(itemOne.getId(),
            itemOne.getEnchantLevel(), itemTwo.getId(), itemTwo.getEnchantLevel());

        // Not implemented or not able to merge!
        if (combinationItem == null)
        {
            player.sendPacket(ExEnchantTwoFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        player.sendPacket(ExEnchantTwoOkPacket.STATIC_PACKET);

        return ValueTask.CompletedTask;
    }
}