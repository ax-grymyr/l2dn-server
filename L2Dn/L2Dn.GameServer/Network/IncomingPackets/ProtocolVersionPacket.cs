using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Logging;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct ProtocolVersionPacket: IIncomingPacket<GameSession>
{
    private int _protocolVersion;

    public void ReadContent(PacketBitReader reader)
    {
        _protocolVersion = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        Logger.Trace($"S({connection.Session.Id})  Protocol version {_protocolVersion}");
        if (_protocolVersion == -2)
        {
            // Ping attempt from the client
            connection.Close();
            return ValueTask.CompletedTask;
        }

        GameSession session = connection.Session;
        session.State = GameSessionState.Authorization;

        bool isProtocolOk = _protocolVersion == session.Config.Protocol.Version;
        int serverId = session.Config.GameServer.Id;

        KeyPacket keyPacket = new(isProtocolOk, serverId, connection.Session.EncryptionKey);
        connection.Send(ref keyPacket);

        return ValueTask.CompletedTask;
    }
}
