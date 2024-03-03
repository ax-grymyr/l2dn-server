using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AttributeChange;

public readonly struct ExChangeAttributeOkPacket: IOutgoingPacket
{
    public static readonly ExChangeAttributeOkPacket STATIC = default;

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_ATTRIBUTE_OK);
    }
}