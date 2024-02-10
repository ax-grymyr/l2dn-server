using System.Collections.Immutable;
using L2Dn.AuthServer.Model;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

internal readonly struct ServerListPacket(ImmutableSortedSet<GameServerInfo> servers, byte? selectedServerId)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        // Check selectedServerId
        byte selectedSrvId = 0;
        if (selectedServerId is not null)
        {
            selectedSrvId = selectedServerId.Value;
            if (servers.All(s => s.ServerId != selectedSrvId))
                selectedSrvId = 0;
        }

        writer.WritePacketCode(OutgoingPacketCodes.ServerList);

        writer.WriteByte((byte)servers.Count);
        writer.WriteByte(selectedSrvId);

        foreach (GameServerInfo serverInfo in servers)
        {
            writer.WriteByte(serverInfo.ServerId); // server id
            writer.WriteInt32(serverInfo.Address);
            writer.WriteInt32(serverInfo.Port);
            writer.WriteByte(serverInfo.AgeLimit); // age limit: 0, 15, 18
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
            writer.WriteByte(serverInfo.ServerId);
            writer.WriteByte(0); // TODO: chars on the server
        }
    }
}