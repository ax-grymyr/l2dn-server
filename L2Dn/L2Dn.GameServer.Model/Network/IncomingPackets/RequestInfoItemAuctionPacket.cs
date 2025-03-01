using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemAuction;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestInfoItemAuctionPacket: IIncomingPacket<GameSession>
{
    private int _instanceId;

    public void ReadContent(PacketBitReader reader)
    {
        _instanceId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canUseItemAuction())
        //     return ValueTask.CompletedTask;

        ItemAuctionInstance? instance = ItemAuctionManager.getInstance().getManagerInstance(_instanceId);
        if (instance == null)
            return ValueTask.CompletedTask;

        ItemAuction? auction = instance.getCurrentAuction();
        if (auction == null)
            return ValueTask.CompletedTask;

        player.updateLastItemAuctionRequest();
        player.sendPacket(new ExItemAuctionInfoPacket(true, auction, instance.getNextAuction()));

        return ValueTask.CompletedTask;
    }
}