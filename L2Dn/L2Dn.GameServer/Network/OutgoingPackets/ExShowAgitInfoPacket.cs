using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
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
            writer.WriteInt32(clanHall.getResidenceId());
            writer.WriteString(clanHall.getOwnerId() <= 0 ? "" : ClanTable.getInstance().getClan(clanHall.getOwnerId()).getName()); // owner clan name
            writer.WriteString(clanHall.getOwnerId() <= 0 ? "" : ClanTable.getInstance().getClan(clanHall.getOwnerId()).getLeaderName()); // leader name
            writer.WriteInt32((int)clanHall.getType()); // Clan hall type
        }
    }
}