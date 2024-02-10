using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal readonly struct RequestLogoutPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        LeaveWorldPacket leaveWorldPacket = new();
        connection.Send(ref leaveWorldPacket, SendPacketOptions.CloseAfterSending);

        return ValueTask.CompletedTask;
    }
}
