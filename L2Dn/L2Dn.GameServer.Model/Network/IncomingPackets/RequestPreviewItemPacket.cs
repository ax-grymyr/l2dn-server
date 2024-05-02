using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPreviewItemPacket: IIncomingPacket<GameSession>
{
    private int _listId;
    private int _count;
    private int[]? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _ = reader.ReadInt32(); // unknown
        _listId = reader.ReadInt32();
        _count = reader.ReadInt32();
        if (_count < 0)
            _count = 0;
        if (_count > 100)
            return; // prevent too long lists
		
        // Create _items table that will contain all ItemID to Wear
        _items = new int[_count];
		
        // Fill _items table with all ItemID to Wear
        for (int i = 0; i < _count; i++)
            _items[i] = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    // Get the current player and return if null
	    Player? player = session.Player;
	    if (player is null || _items is null)
		    return ValueTask.CompletedTask;

	    // TODO: flood protection
	    // if (!client.getFloodProtectors().canPerformTransaction())
	    // {
	    // 	player.sendMessage("You are buying too fast.");
	    // 	return;
	    // }

	    // If Alternate rule Karma punishment is set to true, forbid Wear to player with Karma
	    if (!Config.ALT_GAME_KARMA_PLAYER_CAN_SHOP && player.getReputation() < 0)
		    return ValueTask.CompletedTask;

	    // Check current target of the player and the INTERACTION_DISTANCE
	    WorldObject target = player.getTarget();
	    if (!player.isGM() && (target == null // No target (i.e. GM Shop)
	                           || !(target is Merchant) // Target not a merchant
	                           || !player.IsInsideRadius2D(target, Npc.INTERACTION_DISTANCE) // Distance is too far
	        ))
	    {
		    return ValueTask.CompletedTask;
	    }

	    if (_count < 1 || _listId >= 4000000)
	    {
		    player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }

	    // Get the current merchant targeted by the player
	    Merchant? merchant = target as Merchant;
	    if (merchant == null)
	    {
		    PacketLogger.Instance.Warn(GetType().Name + ": Null merchant!");
		    return ValueTask.CompletedTask;
	    }

	    ProductList buyList = BuyListData.getInstance().getBuyList(_listId);
	    if (buyList == null)
	    {
		    Util.handleIllegalPlayerAction(player,
			    "Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
			    " sent a false BuyList list_id " + _listId, Config.DEFAULT_PUNISH);
		    
		    return ValueTask.CompletedTask;
	    }

	    long totalPrice = 0;
	    Map<int, int> itemList = new();
	    for (int i = 0; i < _count; i++)
	    {
		    int itemId = _items[i];
		    Product product = buyList.getProductByItemId(itemId);
		    if (product == null)
		    {
			    Util.handleIllegalPlayerAction(player,
				    "Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
				    " sent a false BuyList list_id " + _listId + " and item_id " + itemId, Config.DEFAULT_PUNISH);
			    
			    return ValueTask.CompletedTask;
		    }

		    ItemTemplate template = product.getItem();
		    if (template == null)
		    {
			    continue;
		    }

		    int slot = Inventory.getPaperdollIndex(template.getBodyPart());
		    if (slot < 0)
		    {
			    continue;
		    }

		    if (template is Weapon)
		    {
			    if (player.getRace() == Race.KAMAEL)
			    {
				    if (template.getItemType() == WeaponType.NONE)
				    {
					    continue;
				    }
				    
				    if (template.getItemType() == WeaponType.RAPIER ||
				        template.getItemType() == WeaponType.CROSSBOW ||
				        template.getItemType() == WeaponType.ANCIENTSWORD)
				    {
					    continue;
				    }
			    }
		    }
		    else if (template is Armor)
		    {
			    if (player.getRace() == Race.KAMAEL && (template.getItemType() == ArmorType.HEAVY ||
			                                            template.getItemType() == ArmorType.MAGIC))
			    {
				    continue;
			    }
		    }

		    if (itemList.containsKey(slot))
		    {
			    player.sendPacket(SystemMessageId.YOU_CAN_NOT_TRY_THOSE_ITEMS_ON_AT_THE_SAME_TIME);
			    return ValueTask.CompletedTask;
		    }

		    itemList.put(slot, itemId);
		    totalPrice += Config.WEAR_PRICE;
		    if (totalPrice > Inventory.MAX_ADENA)
		    {
			    Util.handleIllegalPlayerAction(player,
				    "Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
				    " tried to purchase over " + Inventory.MAX_ADENA + " adena worth of goods.", Config.DEFAULT_PUNISH);
			    
			    return ValueTask.CompletedTask;
		    }
	    }

	    // Charge buyer and add tax to castle treasury if not owned by npc clan because a Try On is not Free
	    if (totalPrice < 0 || !player.reduceAdena("Wear", totalPrice, player.getLastFolkNPC(), true))
	    {
		    player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
		    return ValueTask.CompletedTask;
	    }

	    if (!itemList.isEmpty())
	    {
		    player.sendPacket(new ShopPreviewInfoPacket(itemList));
		    // Schedule task
		    ThreadPool.schedule(new RemoveWearItemsTask(player), Config.WEAR_DELAY * 1000);
	    }

	    return ValueTask.CompletedTask;
    }

    private sealed class RemoveWearItemsTask(Player player): Runnable
    {
	    public void run()
	    {
		    try
		    {
			    player.sendPacket(SystemMessageId.YOU_ARE_NO_LONGER_TRYING_ON_EQUIPMENT_2);
			    player.sendPacket(new ExUserInfoEquipSlotPacket(player));
		    }
		    catch (Exception e)
		    {
			    PacketLogger.Instance.Warn(GetType().Name + ": " + e);
		    }
	    }
    }
}