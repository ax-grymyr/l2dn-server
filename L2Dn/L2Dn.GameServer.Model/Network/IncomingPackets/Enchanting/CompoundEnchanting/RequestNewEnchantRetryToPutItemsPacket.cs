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

public struct RequestNewEnchantRetryToPutItemsPacket: IIncomingPacket<GameSession>
{
    private int _firstItemObjectId;
    private int _secondItemObjectId;

    public void ReadContent(PacketBitReader reader)
    {
        _firstItemObjectId = reader.ReadInt32();
        _secondItemObjectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.isInStoreMode())
        {
            connection.Send(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_IN_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
            connection.Send(ExEnchantRetryToPutItemFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (player.isProcessingTransaction() || player.isProcessingRequest())
        {
            connection.Send(SystemMessageId.YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP);
            connection.Send(ExEnchantRetryToPutItemFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        CompoundRequest request = new CompoundRequest(player);
        if (!player.addRequest(request))
        {
            connection.Send(ExEnchantRetryToPutItemFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Make sure player owns first item.
        request.setItemOne(_firstItemObjectId);
        Item? itemOne = request.getItemOne();
        if (itemOne == null)
        {
            connection.Send(ExEnchantRetryToPutItemFailPacket.STATIC_PACKET);
            player.removeRequest<CompoundRequest>();
            return ValueTask.CompletedTask;
        }

        // Make sure player owns second item.
        request.setItemTwo(_secondItemObjectId);
        Item? itemTwo = request.getItemTwo();
        if (itemTwo == null)
        {
            connection.Send(ExEnchantRetryToPutItemFailPacket.STATIC_PACKET);
            player.removeRequest<CompoundRequest>();
            return ValueTask.CompletedTask;
        }

        // Not implemented or not able to merge!
        CombinationItem? combinationItem = CombinationItemsData.getInstance().getItemsBySlots(itemOne.Id,
            itemOne.getEnchantLevel(), itemTwo.Id, itemTwo.getEnchantLevel());

        if (combinationItem == null)
        {
            connection.Send(ExEnchantRetryToPutItemFailPacket.STATIC_PACKET);
            player.removeRequest<CompoundRequest>();
            return ValueTask.CompletedTask;
        }

        connection.Send(ExEnchantRetryToPutItemOkPacket.STATIC_PACKET);

        return ValueTask.CompletedTask;
    }
}