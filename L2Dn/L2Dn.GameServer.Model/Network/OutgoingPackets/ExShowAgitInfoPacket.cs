using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Residences;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowAgitInfoPacket: IOutgoingPacket
{
    public static readonly ExShowAgitInfoPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_AGIT_INFO);

        ICollection<ClanHall> clanHalls = ClanHallData.getInstance().getClanHalls();
        writer.WriteInt32(clanHalls.Count);
        foreach (ClanHall clanHall in clanHalls)
        {
            Clan? clan = ClanTable.getInstance().getClan(clanHall.getOwnerId());
            writer.WriteInt32(clanHall.getResidenceId());
            writer.WriteString(clan?.getName() ?? string.Empty); // owner clan name
            writer.WriteString(clan?.getLeaderName() ?? string.Empty); // leader name
            writer.WriteInt32((int)clanHall.getType()); // Clan hall type
        }
    }
}