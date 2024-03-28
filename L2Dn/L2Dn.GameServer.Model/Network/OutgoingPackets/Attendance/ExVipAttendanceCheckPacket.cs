using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Attendance;

public readonly struct ExVipAttendanceCheckPacket: IOutgoingPacket
{
    private readonly bool _available;
	
    public ExVipAttendanceCheckPacket(bool available)
    {
        _available = available;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VIP_ATTENDANCE_CHECK);
        
        writer.WriteByte(_available);
    }
}