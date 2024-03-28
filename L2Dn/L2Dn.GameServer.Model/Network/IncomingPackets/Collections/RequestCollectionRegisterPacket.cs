using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Collections;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Collections;

public struct RequestCollectionRegisterPacket: IIncomingPacket<GameSession>
{
    private int _collectionId;
    private int _index;
    private int _itemObjId;

    public void ReadContent(PacketBitReader reader)
    {
        _collectionId = reader.ReadInt16();
        _index = reader.ReadInt32();
        _itemObjId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Item item = player.getInventory().getItemByObjectId(_itemObjId);
		if (item == null)
		{
			player.sendMessage("Item not found.");
			return ValueTask.CompletedTask;
		}
		
		CollectionDataHolder collection = CollectionData.getInstance().getCollection(_collectionId);
		if (collection == null)
		{
			player.sendMessage("Could not find collection.");
			return ValueTask.CompletedTask;
		}
		
		long count = 0;
		foreach (ItemEnchantHolder data in collection.getItems())
		{
			if (data.getId() == item.getId() && (data.getEnchantLevel() == 0 || data.getEnchantLevel() == item.getEnchantLevel()))
			{
				count = data.getCount();
				break;
			}
		}

		if (count == 0 || item.getCount() < count || item.isEquipped())
		{
			player.sendMessage("Incorrect item count.");
			return ValueTask.CompletedTask;
		}
		
		PlayerCollectionData currentColl = null;
		foreach (PlayerCollectionData coll in player.getCollections())
		{
			if (coll.getCollectionId() == _collectionId)
			{
				currentColl = coll;
				break;
			}
		}
		
		if (currentColl != null && currentColl.getIndex() == _index)
		{
			player.sendPacket(new ExCollectionRegisterPacket(false, _collectionId, _index,
				new ItemEnchantHolder(item.getId(), count, item.getEnchantLevel())));
			
			player.sendPacket(SystemMessageId.THIS_ITEM_CANNOT_BE_ADDED_TO_YOUR_COLLECTION);
			player.sendPacket(new ConfirmDialogPacket("Collection already registered;"));
			return ValueTask.CompletedTask;
		}
		
		player.destroyItem("Collection", item, count, player, true);

		player.sendPacket(new ExCollectionRegisterPacket(true, _collectionId, _index,
			new ItemEnchantHolder(item.getId(), count, item.getEnchantLevel())));
		
		player.getCollections().add(new PlayerCollectionData(_collectionId, item.getId(), _index));
		
		int completeCount = 0;
		foreach (PlayerCollectionData coll in player.getCollections())
		{
			if (coll.getCollectionId() == _collectionId)
			{
				completeCount++;
			}
		}
		
		if (completeCount == collection.getCompleteCount())
		{
			player.sendPacket(new ExCollectionCompletePacket(_collectionId));
			
			// TODO: CollectionData.getInstance().getCollection(_collectionId).getName()
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_COLLECTION_IS_COMPLETED);
			sm.Params.addString("");
			player.sendPacket(sm);
			
			// Apply collection option if all requirements are met.
			Options options = OptionData.getInstance().getOptions(collection.getOptionId());
			if (options != null)
			{
				options.apply(player);
			}
		}
        
        return ValueTask.CompletedTask;
    }
}