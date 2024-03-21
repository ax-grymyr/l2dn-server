using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestRefundItemPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 4; // length of the one item
    private const int CUSTOM_CB_SELL_LIST = 423;
	
    private int _listId;
    private int[]? _items;

    public void ReadContent(PacketBitReader reader)
    {
        _listId = reader.ReadInt32();
        int count = reader.ReadInt32();
        if (count <= 0 || count > Config.MAX_ITEM_IN_PACKET || count * BATCH_LENGTH != reader.Length)
            return;
		
        _items = new int[count];
        for (int i = 0; i < count; i++)
            _items[i] = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are using refund too fast.");
		// 	return ValueTask.CompletedTask;
		// }
		
		if (_items == null || !player.hasRefund())
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		WorldObject target = player.getTarget();
		Merchant merchant = null;
		if (!player.isGM() && _listId != CUSTOM_CB_SELL_LIST)
		{
			if (!(target is Merchant) || !player.isInsideRadius3D(target, Npc.INTERACTION_DISTANCE) ||
			    player.getInstanceId() != target.getInstanceId())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			merchant = (Merchant) target;
		}
		
		if (merchant == null && !player.isGM() && _listId != CUSTOM_CB_SELL_LIST)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		ProductList buyList = BuyListData.getInstance().getBuyList(_listId);
		if (buyList == null)
		{
			Util.handleIllegalPlayerAction(player, "Warning!! Character " + player.getName() + " of account " + player.getAccountName() + " sent a false BuyList list_id " + _listId, Config.DEFAULT_PUNISH);
			return ValueTask.CompletedTask;
		}
		
		if (merchant != null && !buyList.isNpcAllowed(merchant.getId()))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		long weight = 0;
		long adena = 0;
		long slots = 0;
		
		Item[] refund = player.getRefund().getItems().ToArray();
		int[] objectIds = new int[_items.Length];
		for (int i = 0; i < _items.Length; i++)
		{
			int idx = _items[i];
			if (idx < 0 || idx >= refund.Length)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" sent invalid refund index", Config.DEFAULT_PUNISH);
				
				return ValueTask.CompletedTask;
			}
			
			// check for duplicates - indexes
			for (int j = i + 1; j < _items.Length; j++)
			{
				if (idx == _items[j])
				{
					Util.handleIllegalPlayerAction(player,
						"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
						" sent duplicate refund index", Config.DEFAULT_PUNISH);
					
					return ValueTask.CompletedTask;
				}
			}
			
			Item item = refund[idx];
			ItemTemplate template = item.getTemplate();
			objectIds[i] = item.getObjectId();
			
			// second check for duplicates - object ids
			for (int j = 0; j < i; j++)
			{
				if (objectIds[i] == objectIds[j])
				{
					Util.handleIllegalPlayerAction(player,
						"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
						" has duplicate items in refund list", Config.DEFAULT_PUNISH);
					
					return ValueTask.CompletedTask;
				}
			}
			
			long count = item.getCount();
			weight += count * template.getWeight();
			adena += count * (template.getReferencePrice() / 2);
			if (!template.isStackable())
			{
				slots += count;
			}
			else if (player.getInventory().getItemByItemId(template.getId()) == null)
			{
				slots++;
			}
		}
		
		if (weight > int.MaxValue || weight < 0 || !player.getInventory().validateWeight((int) weight))
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		if (slots > int.MaxValue || slots < 0 || !player.getInventory().validateCapacity((int) slots))
		{
			player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		if (!Config.MERCHANT_ZERO_SELL_PRICE && (adena < 0 || !player.reduceAdena("Refund", adena, player.getLastFolkNPC(), false)))
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		for (int i = 0; i < _items.Length; i++)
		{
			Item item = player.getRefund().transferItem("Refund", objectIds[i], long.MaxValue, player.getInventory(),
				player, player.getLastFolkNPC());
			
			if (item == null)
			{
				PacketLogger.Instance.Warn("Error refunding object for char " + player.getName() +
				                           " (newitem == null)");
			}
		}
		
		// Update current load status on player
		player.sendPacket(new ExUserInfoInventoryWeightPacket(player));
		player.sendPacket(new ExBuySellListPacket(player, true));
        
        return ValueTask.CompletedTask;
    }
}