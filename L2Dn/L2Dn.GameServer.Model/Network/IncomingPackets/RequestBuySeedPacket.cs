using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestBuySeedPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 12; // length of the one item
    
    private int _manorId;
    private List<ItemHolder>? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _manorId = reader.ReadInt32();
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
        {
            return;
        }
		
        _items = new(count);
        for (int i = 0; i < count; i++)
        {
            int itemId = reader.ReadInt32();
            long cnt = reader.ReadInt64();
            if (cnt < 1 || itemId < 1)
            {
                _items = null;
                return;
            }
            
            _items.Add(new ItemHolder(itemId, cnt));
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    if (_items == null)
	    {
		    player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		    return ValueTask.CompletedTask;
	    }
	    
	    // TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are buying seeds too fast!");
		// 	return ValueTask.CompletedTask;
		// }
		
		CastleManorManager manor = CastleManorManager.getInstance();
		if (manor.isUnderMaintenance())
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		Castle castle = CastleManager.getInstance().getCastleById(_manorId);
		if (castle == null)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		Npc manager = player.getLastFolkNPC();
		if (!(manager is Merchant) || !manager.canInteract(player) || manager.getParameters().getInt("manor_id", -1) != _manorId)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		long totalPrice = 0;
		long slots = 0;
		long totalWeight = 0;
		
		Map<int, SeedProduction> productInfo = new();
		foreach (ItemHolder ih in _items)
		{
			SeedProduction sp = manor.getSeedProduct(_manorId, ih.getId(), false);
			if (sp == null || sp.getPrice() <= 0 || sp.getAmount() < ih.getCount() || Inventory.MAX_ADENA / ih.getCount() < sp.getPrice())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}
			
			// Calculate price
			totalPrice += sp.getPrice() * ih.getCount();
			if (totalPrice > Inventory.MAX_ADENA)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to purchase over " + Inventory.MAX_ADENA + " adena worth of goods.", Config.DEFAULT_PUNISH);
				
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}
			
			// Calculate weight
			ItemTemplate template = ItemData.getInstance().getTemplate(ih.getId());
			totalWeight += ih.getCount() * template.getWeight();
			
			// Calculate slots
			if (!template.isStackable())
			{
				slots += ih.getCount();
			}
			else if (player.getInventory().getItemByItemId(ih.getId()) == null)
			{
				slots++;
			}
			productInfo.put(ih.getId(), sp);
		}
		
		if (!player.getInventory().validateWeight(totalWeight))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			return ValueTask.CompletedTask;
		}
		
		if (!player.getInventory().validateCapacity(slots))
		{
			player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			return ValueTask.CompletedTask;
		}
		
		if (totalPrice < 0 || player.getAdena() < totalPrice)
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			return ValueTask.CompletedTask;
		}
		
		// Proceed the purchase
		foreach (ItemHolder i in _items)
		{
			SeedProduction sp = productInfo.get(i.getId());
			long price = sp.getPrice() * i.getCount();
			
			// Take Adena and decrease seed amount
			if (!sp.decreaseAmount(i.getCount()) || !player.reduceAdena("Buy", price, player, false))
			{
				// failed buy, reduce total price
				totalPrice -= price;
				continue;
			}
			
			// Add item to player's inventory
			player.addItem("Buy", i.getId(), i.getCount(), manager, true);
		}
		
		// Adding to treasury for Manor Castle
		if (totalPrice > 0)
		{
			castle.addToTreasuryNoTax(totalPrice);
			
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_ADENA_DISAPPEARED);
			sm.Params.addLong(totalPrice);
			player.sendPacket(sm);
			
			if (Config.ALT_MANOR_SAVE_ALL_ACTIONS)
			{
				manor.updateCurrentProduction(_manorId, productInfo.Values);
			}
		}

		return ValueTask.CompletedTask;
    }
}