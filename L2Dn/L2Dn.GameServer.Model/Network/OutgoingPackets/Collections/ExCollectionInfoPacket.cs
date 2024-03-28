using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Collections;

public readonly struct ExCollectionInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _category;
    private readonly Set<int> _collectionIds;
    private readonly List<int> _favoriteIds;
	
    public ExCollectionInfoPacket(Player player, int category)
    {
        _player = player;
        _category = category;
        _collectionIds = new Set<int>();
        foreach (PlayerCollectionData collection in player.getCollections())
        {
            if (CollectionData.getInstance().getCollection(collection.getCollectionId()).getCategory() == category)
            {
                _collectionIds.add(collection.getCollectionId());
            }
        }
        
        _favoriteIds = player.getCollectionFavorites();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_COLLECTION_INFO);

        writer.WriteInt32(_collectionIds.size()); // size
        List<PlayerCollectionData> currentCollection = new();
        foreach (int id in _collectionIds)
        {
            currentCollection.Clear();
            foreach (PlayerCollectionData collection in _player.getCollections())
            {
                if (collection.getCollectionId() == id)
                {
                    currentCollection.add(collection);
                }
            }
			
            writer.WriteInt32(currentCollection.size());
            foreach (PlayerCollectionData collection in currentCollection)
            {
                writer.WriteByte((byte)collection.getIndex());
                writer.WriteInt32(collection.getItemId());
                writer.WriteByte((byte)CollectionData.getInstance().getCollection(id).getItems().get(collection.getIndex()).getEnchantLevel()); // enchant level
                writer.WriteByte(0); // bless
                writer.WriteByte(0); // bless Condition
                writer.WriteInt32(1); // amount
            }
            
            writer.WriteInt16((short)id);
        }
		
        // favoriteList
        writer.WriteInt32(_favoriteIds.size());
        foreach (int id in _favoriteIds)
        {
            writer.WriteInt16((short)id);
        }
		
        // rewardList
        writer.WriteInt32(0);
		
        writer.WriteByte((byte)_category);
        writer.WriteInt16(0);
    }
}