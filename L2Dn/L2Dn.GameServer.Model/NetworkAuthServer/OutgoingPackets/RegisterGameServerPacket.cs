using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model;
using L2Dn.Model;
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

        GameServerAttributes attributes = GameServerAttributes.None;
        if (serverParams.IsTestServer)
            attributes |= GameServerAttributes.PublicTest;

        if (serverParams.ServerType is GameServerType.Classic or GameServerType.Essence)
            attributes |= GameServerAttributes.EssenceOrClassic;
        
        writer.WritePacketCode(OutgoingPacketCodes.RegisterGameServer);
        writer.WriteByte(serverParams.ServerId);
        writer.WriteString(authServerConnection.AccessKey);
        writer.WriteInt32(ipAddress);
        writer.WriteUInt16((ushort)config.ClientListener.Port);
        writer.WriteByte(serverParams.AgeLimit);
        writer.WriteByte(serverParams.IsPvpServer);

        int playerCount = World.getInstance().getPlayers().Count;
        writer.WriteUInt16((ushort)playerCount);
        writer.WriteUInt16((ushort)serverParams.MaxPlayerCount);
        writer.WriteInt32((int)attributes);
        writer.WriteByte(serverParams.Brackets);
    }
}