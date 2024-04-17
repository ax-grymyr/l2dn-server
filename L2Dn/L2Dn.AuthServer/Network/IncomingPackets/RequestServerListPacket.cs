using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.IncomingPackets;

internal struct RequestServerListPacket: IIncomingPacket<AuthSession>
{
    private int _loginKey1;
    private int _loginKey2;

    public void ReadContent(PacketBitReader reader)
    {
        _loginKey1 = reader.ReadInt32();
        _loginKey2 = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, AuthSession session)
    {
        AccountInfo? accountInfo = session.AccountInfo;
        if (_loginKey1 != session.LoginKey1 || _loginKey2 != session.LoginKey2 || accountInfo is null)
        {
            LoginFailPacket loginFailPacket = new(LoginFailReason.AccessDenied);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
            return ValueTask.CompletedTask;
        }

        ServerListPacket serverListPacket = new(GameServerManager.Instance.Servers, accountInfo);
        connection.Send(ref serverListPacket);
        return ValueTask.CompletedTask;
    }
}