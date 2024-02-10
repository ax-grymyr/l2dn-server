using L2Dn.AuthServer.Db;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer.IncomingPackets;

internal struct RegisterGameServerPacket: IIncomingPacket<GameServerSession>
{
    private byte _serverId;
    private string? _token;
    private uint _ipAddress;
    private ushort _port;
    private byte _ageLimit;
    private bool _isPvpServer;
    private ushort _playerCount;
    private ushort _maxPlayerCount;
    private bool _isOnline;
    private GameServerAttributes _attributes;
    private bool _brackets;

    public void ReadContent(PacketBitReader reader)
    {
        _serverId = reader.ReadByte();
        _token = reader.ReadString();
        _ipAddress = reader.ReadUInt32();
        _port = reader.ReadUInt16();
        _ageLimit = reader.ReadByte();
        _isPvpServer = reader.ReadBoolean();
        _playerCount = reader.ReadUInt16();
        _maxPlayerCount = reader.ReadUInt16();
        _isOnline = reader.ReadBoolean();
        _attributes = reader.ReadValue<GameServerAttributes>();
        _brackets = reader.ReadBoolean();
    }

    public async ValueTask ProcessAsync(Connection<GameServerSession> connection)
    {
        
    }
}