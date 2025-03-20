using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeInfoPacket: IOutgoingPacket
{
    private readonly Clan _clan;

    public PledgeInfoPacket(Clan clan)
    {
        _clan = clan;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_INFO);

        writer.WriteInt32(ServerConfig.Instance.GameServerParams.ServerId);
        writer.WriteInt32(_clan.Id);
        writer.WriteString(_clan.getName());
        writer.WriteString(_clan.getAllyName());
    }
}