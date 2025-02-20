using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemAuction;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestBidItemAuctionPacket: IIncomingPacket<GameSession>
{
    private int _instanceId;
    private long _bid;

    public void ReadContent(PacketBitReader reader)
    {
        _instanceId = reader.ReadInt32();
        _bid = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canPerformTransaction())
        // {
        //     player.sendMessage("You are bidding too fast.");
        //     return ValueTask.CompletedTask;
        // }

        if (_bid < 0 || _bid > Inventory.MAX_ADENA)
            return ValueTask.CompletedTask;

        ItemAuctionInstance? instance = ItemAuctionManager.getInstance().getManagerInstance(_instanceId);
        if (instance != null)
        {
            ItemAuction auction = instance.getCurrentAuction();
            if (auction != null)
            {
                auction.registerBid(player, _bid);
            }
        }

        return ValueTask.CompletedTask;
    }
}