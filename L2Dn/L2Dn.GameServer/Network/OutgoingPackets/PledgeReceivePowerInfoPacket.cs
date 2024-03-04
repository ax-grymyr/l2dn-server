using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeReceivePowerInfoPacket: IOutgoingPacket
{
    private readonly ClanMember _member;
	
    public PledgeReceivePowerInfoPacket(ClanMember member)
    {
        _member = member;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_RECEIVE_POWER_INFO);
        
        writer.WriteInt32(_member.getPowerGrade()); // power grade
        writer.WriteString(_member.getName());
        writer.WriteInt32((int)_member.getClan().getRankPrivs(_member.getPowerGrade())); // privileges
    }
}