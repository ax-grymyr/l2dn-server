using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.NewHenna;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct RequestNewHennaListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        NewHennaListPacket newHennaListPacket = new NewHennaListPacket(player, 0);
        connection.Send(ref newHennaListPacket);
        
        return ValueTask.CompletedTask;
    }
}