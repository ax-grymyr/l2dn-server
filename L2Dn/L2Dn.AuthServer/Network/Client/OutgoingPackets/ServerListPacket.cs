using L2Dn.AuthServer.Model;
using L2Dn.Logging;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

/// <summary>
/// 0x04 - ServerList
/// </summary>
internal readonly struct ServerListPacket(List<GameServerInfo> servers, int? selectedServerId): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        Span<byte> addressBytes = stackalloc byte[4];

        writer.WriteByte(0x04); // packet code

        writer.WriteByte((byte)servers.Count);
        writer.WriteByte((byte)(selectedServerId ?? 0));

        foreach (GameServerInfo serverInfo in servers)
        {
            writer.WriteByte((byte)serverInfo.ServerId); // server id

            if (serverInfo.Address.TryWriteBytes(addressBytes, out int bytesWritten) && bytesWritten == 4)
            {
                writer.WriteByte(addressBytes[0]);
                writer.WriteByte(addressBytes[1]);
                writer.WriteByte(addressBytes[2]);
                writer.WriteByte(addressBytes[3]);
            }
            else
            {
                Logger.Error($"Error converting IP address {serverInfo.Address} to bytes");
                writer.WriteByte(127);
                writer.WriteByte(0);
                writer.WriteByte(0);
                writer.WriteByte(1);
            }
            
            writer.WriteInt32(serverInfo.Port);
            writer.WriteByte(0x0f); // age limit: 0, 15, 18
            writer.WriteBoolean(serverInfo.IsPvpServer);
            writer.WriteUInt16((ushort)serverInfo.PlayerCount);
            writer.WriteUInt16((ushort)serverInfo.MaxPlayerCount);
            writer.WriteBoolean(serverInfo.IsOnline);

            writer.WriteInt32((int)serverInfo.Attributes);
            writer.WriteBoolean(serverInfo.Brackets);
        }

        writer.WriteInt16(0xA4); // unknown

        // Chars on servers
        foreach (GameServerInfo serverInfo in servers)
        {
            writer.WriteByte((byte)serverInfo.ServerId);
            writer.WriteByte(0); // TODO: chars on the server
        }
    }
}
