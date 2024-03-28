using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeRecruitBoardDetailPacket: IOutgoingPacket
{
    private readonly PledgeRecruitInfo _pledgeRecruitInfo;
	
    public ExPledgeRecruitBoardDetailPacket(PledgeRecruitInfo pledgeRecruitInfo)
    {
        _pledgeRecruitInfo = pledgeRecruitInfo;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_RECRUIT_BOARD_DETAIL);
        
        writer.WriteInt32(_pledgeRecruitInfo.getClanId());
        writer.WriteInt32(_pledgeRecruitInfo.getKarma());
        writer.WriteString(_pledgeRecruitInfo.getInformation());
        writer.WriteString(_pledgeRecruitInfo.getDetailedInformation());
        writer.WriteInt32(_pledgeRecruitInfo.getApplicationType());
        writer.WriteInt32(_pledgeRecruitInfo.getRecruitType());
    }
}