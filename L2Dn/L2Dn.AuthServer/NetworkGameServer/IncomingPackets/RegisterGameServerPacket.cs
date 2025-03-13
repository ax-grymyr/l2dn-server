using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer.IncomingPackets;

internal struct RegisterGameServerPacket: IIncomingPacket<GameServerSession>
{
    private byte _serverId;
    private string? _accessKey;
    private int _ipAddress;
    private ushort _port;
    private byte _ageLimit;
    private bool _isPvpServer;
    private ushort _playerCount;
    private ushort _maxPlayerCount;
    private GameServerAttributes _attributes;
    private bool _brackets;

    public void ReadContent(PacketBitReader reader)
    {
        _serverId = reader.ReadByte();
        _accessKey = reader.ReadString();
        _ipAddress = reader.ReadInt32();
        _port = reader.ReadUInt16();
        _ageLimit = reader.ReadByte();
        _isPvpServer = reader.ReadBoolean();
        _playerCount = reader.ReadUInt16();
        _maxPlayerCount = reader.ReadUInt16();
        _attributes = reader.ReadEnum<GameServerAttributes>();
        _brackets = reader.ReadBoolean();
    }

    public ValueTask ProcessAsync(Connection connection, GameServerSession session)
    {
        RegistrationResult result = GameServerManager.Instance.RegisterServer(new GameServerInfo
        {
            ServerId = _serverId,
            Address = _ipAddress,
            Port = _port,
            AgeLimit = _ageLimit,
            IsPvpServer = _isPvpServer,
            PlayerCount = _playerCount,
            MaxPlayerCount = _maxPlayerCount,
            Attributes = _attributes,
            Brackets = _brackets,
            AccessKey = _accessKey
        });

        GameServerInfo? serverInfo = GameServerManager.Instance.GetServerInfo(_serverId);
        session.ServerInfo = serverInfo;
        if (serverInfo is not null)
            serverInfo.Connection = connection;

        RegistrationResultPacket registrationResultPacket = new(result);
        connection.Send(ref registrationResultPacket,
            result != RegistrationResult.Success ? SendPacketOptions.CloseAfterSending : SendPacketOptions.None);

        return ValueTask.CompletedTask;
    }
}