using System.Collections.Immutable;
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

public struct RequestNewEnchantPushOnePacket: IIncomingPacket<GameSession>
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

        CompoundRequest request = new CompoundRequest(player);
        if (!player.addRequest(request))
        {
            player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        // Make sure player owns this item.
        request.setItemOne(_objectId);
        Item itemOne = request.getItemOne();
        if (itemOne == null)
        {
            player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
            player.removeRequest<CompoundRequest>();
            return ValueTask.CompletedTask;
        }

        ImmutableArray<CombinationItem> combinationItems = CombinationItemsData.getInstance()
            .getItemsByFirstSlot(itemOne.getId(), itemOne.getEnchantLevel());

        // Not implemented or not able to merge!
        if (combinationItems.IsDefaultOrEmpty)
        {
            player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
            player.removeRequest<CompoundRequest>();
            return ValueTask.CompletedTask;
        }

        player.sendPacket(ExEnchantOneOkPacket.STATIC_PACKET);

        return ValueTask.CompletedTask;
    }
}