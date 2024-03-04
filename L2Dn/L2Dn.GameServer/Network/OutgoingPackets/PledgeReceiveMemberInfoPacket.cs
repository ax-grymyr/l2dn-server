using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeReceiveMemberInfoPacket: IOutgoingPacket
{
    private readonly ClanMember _member;
	
    public PledgeReceiveMemberInfoPacket(ClanMember member)
    {
        _member = member;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_RECEIVE_MEMBER_INFO);
        
        writer.WriteInt32(_member.getPledgeType());
        writer.WriteString(_member.getName());
        writer.WriteString(_member.getTitle()); // title
        writer.WriteInt32(_member.getPowerGrade()); // power
        // clan or subpledge name
        if (_member.getPledgeType() != 0)
        {
            writer.WriteString((_member.getClan().getSubPledge(_member.getPledgeType())).getName());
        }
        else
        {
            writer.WriteString(_member.getClan().getName());
        }
        
        writer.WriteString(_member.getApprenticeOrSponsorName()); // name of this member's apprentice/sponsor
    }
}