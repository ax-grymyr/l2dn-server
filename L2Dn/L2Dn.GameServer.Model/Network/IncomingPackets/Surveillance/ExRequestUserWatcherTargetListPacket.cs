using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Surveillance;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Surveillance;

public struct ExRequestUserWatcherTargetListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExUserWatcherTargetListPacket(player));
        
        return ValueTask.CompletedTask;
    }
}