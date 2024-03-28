using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.OutgoingPackets.Blessing;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Blessing;

public struct RequestBlessOptionCancelPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.removeRequest<BlessingItemRequest>();
        player.sendPacket(new ExBlessOptionCancelPacket(1));
        
        return ValueTask.CompletedTask;
    }
}