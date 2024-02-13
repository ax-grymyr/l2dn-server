using L2Dn.AuthServer.Model;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer.IncomingPackets;

internal struct UpdateStatusPacket: IIncomingPacket<GameServerSession>
{
    private ushort _playerCount;

    public void ReadContent(PacketBitReader reader)
    {
        _playerCount = reader.ReadUInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameServerSession session)
    {
        GameServerInfo? serverInfo = session.ServerInfo;
        if (serverInfo is null)
        {
            connection.Close();
            return ValueTask.CompletedTask;
        }

        serverInfo.PlayerCount = _playerCount;
        return ValueTask.CompletedTask;
    }
}