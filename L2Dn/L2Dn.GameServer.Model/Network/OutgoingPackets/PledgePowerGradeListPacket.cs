using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgePowerGradeListPacket: IOutgoingPacket
{
    private readonly ICollection<Clan.RankPrivs> _privs;
	
    public PledgePowerGradeListPacket(ICollection<Clan.RankPrivs> privs)
    {
        _privs = privs;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_POWER_GRADE_LIST);
        
        writer.WriteInt32(_privs.Count);
        foreach (Clan.RankPrivs temp in _privs)
        {
            writer.WriteInt32(temp.getRank());
            writer.WriteInt32(temp.getParty());
        }
    }
}