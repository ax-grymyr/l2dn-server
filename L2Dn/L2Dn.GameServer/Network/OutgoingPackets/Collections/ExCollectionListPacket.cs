using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Collections;

public readonly struct ExCollectionListPacket: IOutgoingPacket
{
    private readonly int _category;
	
    public ExCollectionListPacket(int category)
    {
        _category = category;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_COLLECTION_LIST);

        writer.WriteByte((byte)_category);
        writer.WriteInt32(0); // size & loop body
    }
}