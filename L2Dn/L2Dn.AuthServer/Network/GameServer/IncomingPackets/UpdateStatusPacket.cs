using L2Dn.AuthServer.Model;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer.IncomingPackets;

internal struct UpdateStatusPacket: IIncomingPacket<GameServerSession>
{
    private ushort _playerCount;

    public void ReadContent(PacketBitReader reader)
    {
        _playerCount = reader.ReadUInt16();
    }

    public ValueTask ProcessAsync(Connection<GameServerSession> connection)
    {
        GameServerInfo? serverInfo = connection.Session.ServerInfo;
        if (serverInfo is null)
        {
            connection.Close();
            return ValueTask.CompletedTask;
        }

        serverInfo.PlayerCount = _playerCount;
        return ValueTask.CompletedTask;
    }
}