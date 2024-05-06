using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.CompoundEnchant;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.CompoundEnchanting;

public struct RequestNewEnchantTryPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    if (player.isInStoreMode())
	    {
		    player.sendPacket(SystemMessageId.YOU_CANNOT_DO_THAT_WHILE_IN_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
		    player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    if (player.isProcessingTransaction() || player.isProcessingRequest())
	    {
		    player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP);
		    player.sendPacket(ExEnchantOneFailPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    CompoundRequest request = player.getRequest<CompoundRequest>();
	    if (request == null || request.isProcessing())
	    {
		    player.sendPacket(ExEnchantFailPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    request.setProcessing(true);

	    Item itemOne = request.getItemOne();
	    Item itemTwo = request.getItemTwo();
	    if (itemOne == null || itemTwo == null)
	    {
		    player.sendPacket(ExEnchantFailPacket.STATIC_PACKET);
		    player.removeRequest<CompoundRequest>();
		    return ValueTask.CompletedTask;
	    }

	    // Lets prevent using same item twice. Also stackable item check.
	    if (itemOne.getObjectId() == itemTwo.getObjectId() && (!itemOne.isStackable() ||
	                                                           player.getInventory()
		                                                           .getInventoryItemCount(
			                                                           itemOne.getTemplate().getId(), -1) < 2))
	    {
		    player.sendPacket(new ExEnchantFailPacket(itemOne.getId(), itemTwo.getId()));
		    player.removeRequest<CompoundRequest>();
		    return ValueTask.CompletedTask;
	    }

	    CombinationItem combinationItem = CombinationItemsData.getInstance().getItemsBySlots(itemOne.getId(),
		    itemOne.getEnchantLevel(), itemTwo.getId(), itemTwo.getEnchantLevel());

	    // Not implemented or not able to merge!
	    if (combinationItem == null)
	    {
		    player.sendPacket(new ExEnchantFailPacket(itemOne.getId(), itemTwo.getId()));
		    player.removeRequest<CompoundRequest>();
		    return ValueTask.CompletedTask;
	    }

	    if (combinationItem.getCommission() > player.getAdena())
	    {
		    player.sendPacket(new ExEnchantFailPacket(itemOne.getId(), itemTwo.getId()));
		    player.removeRequest<CompoundRequest>();
		    player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
		    return ValueTask.CompletedTask;
	    }

	    // Calculate compound result.
	    double random = Rnd.nextDouble() * 100;
	    bool success = random <= combinationItem.getChance();
	    CombinationItemReward rewardItem = combinationItem.getReward(success);

	    // Add item (early).
	    Item item = player.addItem("Compound-Result", rewardItem.getId(), rewardItem.getCount(),
		    rewardItem.getEnchantLevel(), null, true);

	    // Send success or fail packet.
	    if (success)
	    {
		    player.sendPacket(new ExEnchantSuccessPacket(item.getId()));
		    if (combinationItem.isAnnounce())
		    {
			    Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item, ExItemAnnouncePacket.COMPOUND));
		    }
	    }
	    else
	    {
		    player.sendPacket(new ExEnchantFailPacket(item.getId(), itemTwo.getId()));
	    }

	    // Take required items.
	    if (player.destroyItem("Compound-Item-One", itemOne, 1, null, true) &&
	        player.destroyItem("Compound-Item-Two", itemTwo, 1, null, true) &&
	        (combinationItem.getCommission() <= 0 ||
	         player.reduceAdena("Compound-Commission", combinationItem.getCommission(), player, true)))
	    {
		    List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
		    itemsToUpdate.Add(new ItemInfo(item, ItemChangeType.MODIFIED));
		    if (itemOne.isStackable() && itemOne.getCount() > 0)
		    {
			    itemsToUpdate.Add(new ItemInfo(itemOne, ItemChangeType.MODIFIED));
		    }
		    else
		    {
			    itemsToUpdate.Add(new ItemInfo(itemOne, ItemChangeType.REMOVED));
		    }

		    if (itemTwo.isStackable() && itemTwo.getCount() > 0)
		    {
			    itemsToUpdate.Add(new ItemInfo(itemTwo, ItemChangeType.MODIFIED));
		    }
		    else
		    {
			    itemsToUpdate.Add(new ItemInfo(itemTwo, ItemChangeType.REMOVED));
		    }

		    InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
		    player.sendInventoryUpdate(iu);
	    }

	    player.removeRequest<CompoundRequest>();

	    return ValueTask.CompletedTask;
    }
}