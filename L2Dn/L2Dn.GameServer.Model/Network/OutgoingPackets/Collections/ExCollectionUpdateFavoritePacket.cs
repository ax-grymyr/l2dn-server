using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Collections;

public readonly struct ExCollectionUpdateFavoritePacket: IOutgoingPacket
{
    private readonly int _isAdd;
    private readonly int _collectionId;
	
    public ExCollectionUpdateFavoritePacket(int isAdd, int collectionId)
    {
        _isAdd = isAdd;
        _collectionId = collectionId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_COLLECTION_UPDATE_FAVORITE);

        writer.WriteByte((byte)_isAdd);
        writer.WriteInt16((short)_collectionId);
    }
}