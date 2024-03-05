using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct ProtocolVersionPacket: IIncomingPacket<GameSession>
{
    private int _protocolVersion;

    public void ReadContent(PacketBitReader reader)
    {
        _protocolVersion = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (_protocolVersion == -2)
        {
            // Ping attempt from the client
            connection.Close();
            return ValueTask.CompletedTask;
        }

        session.State = GameSessionState.Authorization;

        bool isProtocolOk = Config.PROTOCOL_LIST.Contains(_protocolVersion);
        session.ProtocolVersion = _protocolVersion;
        session.IsProtocolOk = isProtocolOk;
        int serverId = session.Config.GameServerParams.ServerId;

        KeyPacket keyPacket = new(isProtocolOk, serverId, session.EncryptionKey);
        connection.Send(ref keyPacket, isProtocolOk ? SendPacketOptions.None : SendPacketOptions.CloseAfterSending);

        return ValueTask.CompletedTask;
    }
}
