using L2Dn.AuthServer.Network.Client.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.IncomingPackets;

internal struct RequestGGAuthPacket: IIncomingPacket<AuthSession>
{
    private int _sessionId;

    public void ReadContent(PacketBitReader reader)
    {
        _sessionId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection<AuthSession> connection)
    {
        if (connection.Session.Id != _sessionId)
        {
            LoginFailPacket loginFailPacket = new(LoginFailReason.AccessDenied);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
        }
        else
        {
            GGAuthPacket ggAuthPacket = new(0x0B);
            connection.Send(ref ggAuthPacket);
        }
        
        return ValueTask.CompletedTask;
    }
}
