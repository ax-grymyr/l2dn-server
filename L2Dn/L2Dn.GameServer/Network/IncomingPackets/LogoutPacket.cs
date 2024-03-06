using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public readonly struct LogoutPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        LeaveWorldPacket leaveWorldPacket = new();
        connection.Send(ref leaveWorldPacket, SendPacketOptions.CloseAfterSending);

        return ValueTask.CompletedTask;
    }
}
