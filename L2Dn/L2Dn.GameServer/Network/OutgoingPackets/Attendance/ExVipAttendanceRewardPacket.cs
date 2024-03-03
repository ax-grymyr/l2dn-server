using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Attendance;

public readonly struct ExVipAttendanceRewardPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VIP_ATTENDANCE_REWARD);

        writer.WriteByte(1);
    }
}