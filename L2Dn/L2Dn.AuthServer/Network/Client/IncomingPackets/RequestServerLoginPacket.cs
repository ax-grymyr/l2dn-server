using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.Network.Client.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.IncomingPackets;

internal struct RequestServerLoginPacket: IIncomingPacket<AuthSession>
{
    private int _loginKey1;
    private int _loginKey2;
    private byte _serverId;

    public void ReadContent(PacketBitReader reader)
    {
        _loginKey1 = reader.ReadInt32();
        _loginKey2 = reader.ReadInt32();
        _serverId = reader.ReadByte();
    }

    public async ValueTask ProcessAsync(Connection<AuthSession> connection)
    {
        AuthSession session = connection.Session;
        AccountInfo? accountInfo = session.AccountInfo;
        
        byte serverId = _serverId;
        GameServerInfo? serverInfo = GameServerManager.Instance.Servers.SingleOrDefault(x => x.ServerId == serverId);
        
        if (_loginKey1 != session.LoginKey1 || _loginKey2 != session.LoginKey2 || accountInfo is null || 
            serverInfo is null || !serverInfo.IsOnline)
        {
            PlayFailPacket loginFailPacket = new(PlayFailReason.AccessFailed);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
            return;
        }

        if (serverId != accountInfo.LastServerId)
        {
            accountInfo.LastServerId = serverInfo.ServerId;
            await AccountManager.Instance.UpdateSelectedGameServerAsync(accountInfo.AccountId, serverId);
        }
        
        if (serverInfo.PlayerCount >= serverInfo.MaxPlayerCount)
        {
            PlayFailPacket loginFailPacket = new(PlayFailReason.TooManyPlayers);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
            return;
        }
        
        // TODO send login data to game server
        
        PlayOkPacket playOkPacket = new(session.PlayKey1, session.PlayKey2);
        connection.Send(ref playOkPacket, SendPacketOptions.CloseAfterSending);
    }
}
