using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeRecruitInfoPacket: IOutgoingPacket
{
    private readonly Clan _clan;
	
    public ExPledgeRecruitInfoPacket(int clanId)
    {
        _clan = ClanTable.getInstance().getClan(clanId);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_RECRUIT_INFO);
        
        ICollection<Clan.SubPledge> subPledges = _clan.getAllSubPledges();
        writer.WriteString(_clan.getName());
        writer.WriteString(_clan.getLeaderName());
        writer.WriteInt32(_clan.getLevel());
        writer.WriteInt32(_clan.getMembersCount());
        writer.WriteInt32(subPledges.Count);
        foreach (Clan.SubPledge subPledge in subPledges)
        {
            writer.WriteInt32(subPledge.getId());
            writer.WriteString(subPledge.getName());
        }
    }
}