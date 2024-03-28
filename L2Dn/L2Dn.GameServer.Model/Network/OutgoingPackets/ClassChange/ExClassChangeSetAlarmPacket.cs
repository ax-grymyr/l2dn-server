using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.ClassChange;

public readonly struct ExClassChangeSetAlarmPacket: IOutgoingPacket
{
    public static readonly ExClassChangeSetAlarmPacket STATIC_PACKET = default;
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CLASS_CHANGE_SET_ALARM);
    }
}