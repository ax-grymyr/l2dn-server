using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBirthdayPopupPacket: IOutgoingPacket
{
    public static readonly ExBirthdayPopupPacket STATIC_PACKET = new();
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NOTIFY_BIRTH_DAY);
    }
}