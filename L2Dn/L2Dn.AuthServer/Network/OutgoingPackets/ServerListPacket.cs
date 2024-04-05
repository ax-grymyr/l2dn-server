using L2Dn.AuthServer.Model;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal readonly struct ServerListPacket(ReadOnlyMemory<GameServerInfo> servers, byte? selectedServerId)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        ReadOnlySpan<GameServerInfo> serversSpan = servers.Span; 

        // Check selectedServerId
        byte selectedSrvId = 0;
        if (selectedServerId is not null && !serversSpan.IsEmpty)
        {
            selectedSrvId = serversSpan[0].ServerId;
            for (int i = 1; i < serversSpan.Length; i++)
            {
                if (serversSpan[i].ServerId == selectedServerId.Value)
                    selectedSrvId = serversSpan[i].ServerId;
            }
        }

        writer.WritePacketCode(OutgoingPacketCodes.ServerList);

        writer.WriteByte((byte)servers.Length);
        writer.WriteByte(selectedSrvId);

        foreach (GameServerInfo serverInfo in serversSpan)
        {
            writer.WriteByte(serverInfo.ServerId); // server id
            writer.WriteInt32(serverInfo.Address);
            writer.WriteInt32(serverInfo.Port);
            writer.WriteByte(serverInfo.AgeLimit); // age limit: 0, 15, 18
            writer.WriteByte(serverInfo.IsPvpServer);
            writer.WriteUInt16((ushort)serverInfo.PlayerCount);
            writer.WriteUInt16((ushort)serverInfo.MaxPlayerCount);
            writer.WriteByte(serverInfo.IsOnline);

            writer.WriteInt32((int)serverInfo.Attributes);
            writer.WriteByte(serverInfo.Brackets);
        }

        writer.WriteInt16(0xA4); // unknown

        // Chars on servers
        foreach (GameServerInfo serverInfo in serversSpan)
        {
            writer.WriteByte(serverInfo.ServerId);
            writer.WriteByte(0); // TODO: chars on the server
        }
    }
}