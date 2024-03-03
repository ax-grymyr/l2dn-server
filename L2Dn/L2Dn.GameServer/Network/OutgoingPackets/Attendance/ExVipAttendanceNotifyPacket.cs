using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Attendance;

public readonly struct ExVipAttendanceNotifyPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VIP_ATTENDANCE_NOTIFY);

        writer.WriteByte(1);
    }
}