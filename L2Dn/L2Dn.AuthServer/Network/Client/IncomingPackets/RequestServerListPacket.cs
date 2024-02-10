using L2Dn.AuthServer.Network.Client.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.IncomingPackets;

internal struct RequestServerListPacket: IIncomingPacket<AuthSession>
{
    private int _loginKey1;
    private int _loginKey2;

    public void ReadContent(PacketBitReader reader)
    {
        _loginKey1 = reader.ReadInt32();
        _loginKey2 = reader.ReadInt32();
    }

    public async ValueTask ProcessAsync(Connection<AuthSession> connection)
    {
        AuthSession session = connection.Session;
        if (_loginKey1 != session.LoginKey1 || _loginKey2 != session.LoginKey2)
        {
            LoginFailPacket loginFailPacket = new(LoginFailReason.AccessDenied);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
            return;
        }

        await session.UpdateGameServerListAsync();
        if (session.SelectedGameServerId is null && session.GameServers.Count!=0)
            session.SelectedGameServerId = session.GameServers[0].ServerId;
        
        ServerListPacket serverListPacket = new(session.GameServers, session.SelectedGameServerId);
        connection.Send(ref serverListPacket);
    }
}
