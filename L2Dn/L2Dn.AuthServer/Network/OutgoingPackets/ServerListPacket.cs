using System.Collections.Immutable;
using L2Dn.AuthServer.Model;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal readonly struct ServerListPacket(ImmutableArray<GameServerInfo> servers, AccountInfo accountInfo)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        byte selectedServerId = 0;
        if (servers.Length != 0)
        {
            selectedServerId = servers[0].ServerId;
            for (int i = 1; i < servers.Length; i++)
            {
                if (servers[i].ServerId == accountInfo.LastServerId)
                    selectedServerId = servers[i].ServerId;
            }
        }

        writer.WritePacketCode(OutgoingPacketCodes.ServerList);

        writer.WriteByte((byte)servers.Length);
        writer.WriteByte(selectedServerId);
        
        foreach (GameServerInfo serverInfo in servers)
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
        foreach (GameServerInfo serverInfo in servers)
        {
            writer.WriteByte(serverInfo.ServerId);
            writer.WriteByte(accountInfo.CharacterCount[serverInfo.ServerId]);
        }
    }
}