using System.Collections.Immutable;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Collections;

public readonly struct ExCollectionActiveEventPacket: IOutgoingPacket
{
    private readonly ImmutableArray<CollectionDataHolder> _collections;

    public ExCollectionActiveEventPacket()
    {
        _collections = CollectionData.getInstance().getCollectionsByTabId(7);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_COLLECTION_ACTIVE_EVENT);

        writer.WriteInt32(_collections.Length);
        foreach (CollectionDataHolder collection in _collections)
            writer.WriteInt16((short)collection.getCollectionId());
    }
}