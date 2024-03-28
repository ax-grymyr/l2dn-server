using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Collections;

public readonly struct ExCollectionCompletePacket: IOutgoingPacket
{
    private readonly int _collectionId;
	
    public ExCollectionCompletePacket(int collectionId)
    {
        _collectionId = collectionId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_COLLECTION_COMPLETE);

        writer.WriteInt16((short)_collectionId);
        writer.WriteInt32(0);
    }
}