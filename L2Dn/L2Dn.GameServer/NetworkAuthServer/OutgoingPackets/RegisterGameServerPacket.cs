using L2Dn.GameServer.Configuration;
using L2Dn.Packets;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;

internal readonly struct RegisterGameServerPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(RegisterGameServerPacket));
    
    public void WriteContent(PacketBitWriter writer)
    {
        ServerConfig config = ServerConfig.Instance;
        AuthServerConnectionConfig authServerConnection = config.AuthServerConnection;
        GameServerParamsConfig serverParams = config.GameServerParams;

        int ipAddress;
        try
        {
            ipAddress = IPAddressUtil.ConvertIP4AddressToInt(authServerConnection.PublishAddress);
        }
        catch (ArgumentException)
        {
            _logger.Error($"Could not convert publish IP address '{authServerConnection.PublishAddress}' to integer");
            ipAddress = IPAddressUtil.Loopback;
        }

        GameServerAttributes attributes = GameServerAttributes.Normal; // todo
        if (serverParams.IsTestServer)
            attributes |= GameServerAttributes.PublicTest;
        
        writer.WritePacketCode(OutgoingPacketCodes.RegisterGameServer);
        writer.WriteByte(serverParams.ServerId);
        writer.WriteString(authServerConnection.AccessKey);
        writer.WriteInt32(ipAddress);
        writer.WriteUInt16((ushort)config.ClientListener.Port);
        writer.WriteByte(serverParams.AgeLimit);
        writer.WriteBoolean(serverParams.IsPvpServer);
        writer.WriteUInt16(0); // todo: playerCount
        writer.WriteUInt16((ushort)serverParams.MaxPlayerCount);
        writer.WriteEnum(attributes);
        writer.WriteBoolean(serverParams.Brackets);
    }

    [Flags]
    private enum GameServerAttributes
    {
        None = 0,
        Normal = 1,
        Relax = 2,
        PublicTest = 4,
        NoLabel = 8,
        CharacterCreationRestricted = 16,
        Event = 32,
        Free = 64,
    }
}