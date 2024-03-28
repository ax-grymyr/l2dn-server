using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeWaitingUserPacket: IOutgoingPacket
{
    private readonly PledgeApplicantInfo _pledgeRecruitInfo;
	
    public ExPledgeWaitingUserPacket(PledgeApplicantInfo pledgeRecruitInfo)
    {
        _pledgeRecruitInfo = pledgeRecruitInfo;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_WAITING_USER);
        
        writer.WriteInt32(_pledgeRecruitInfo.getPlayerId());
        writer.WriteString(_pledgeRecruitInfo.getMessage());
    }
}