using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

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

        writer.WriteInt32(Config.SERVER_ID);
        writer.WriteInt32(_clan.getId());
        writer.WriteString(_clan.getName());
        writer.WriteString(_clan.getAllyName());
    }
}