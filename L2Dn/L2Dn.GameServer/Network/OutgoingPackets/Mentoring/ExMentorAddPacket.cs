using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Mentoring;

public readonly struct ExMentorAddPacket: IOutgoingPacket
{
    private readonly Player _mentor;
	
    public ExMentorAddPacket(Player mentor)
    {
        _mentor = mentor;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MENTOR_ADD);
        
        writer.WriteString(_mentor.getName());
        writer.WriteInt32((int)_mentor.getActiveClass());
        writer.WriteInt32(_mentor.getLevel());
    }
}