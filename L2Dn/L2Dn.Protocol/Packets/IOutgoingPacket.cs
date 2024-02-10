namespace L2Dn.Packets;

public interface IOutgoingPacket
{
    void WriteContent(PacketBitWriter writer);
}
