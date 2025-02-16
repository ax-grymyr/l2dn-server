using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestSellItemPacket: IIncomingPacket<GameSession>
{
    private const int BATCH_LENGTH = 16;
    private const int CUSTOM_CB_SELL_LIST = 423;
	
    private int _listId;
    private List<UniqueItemHolder> _items;

    public void ReadContent(PacketBitReader reader)
    {
        _listId = reader.ReadInt32();
        int size = reader.ReadInt32();
        if (size <= 0 || size > Config.MAX_ITEM_IN_PACKET || size * BATCH_LENGTH != reader.Length)
            return;
        
        _items = new(size);
        for (int i = 0; i < size; i++)
        {
            int objectId = reader.ReadInt32();
            int itemId = reader.ReadInt32();
            long count = reader.ReadInt64();
            if (objectId < 1 || itemId < 1 || count < 1)
            {
                _items = null;
                return;
            }
            
            _items.Add(new UniqueItemHolder(itemId, objectId, count));
        }
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformTransaction())
		// {
		// 	player.sendMessage("You are buying too fast.");
		// 	return;
		// }
		
		if (_items == null)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		// Alt game - Karma punishment
		if (!Config.ALT_GAME_KARMA_PLAYER_CAN_SHOP && player.getReputation() < 0)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		WorldObject target = player.getTarget();
		Merchant? merchant = null;
		if (!player.isGM() && _listId != CUSTOM_CB_SELL_LIST)
		{
			if (target == null || !player.IsInsideRadius3D(target, Npc.INTERACTION_DISTANCE) ||
			    player.getInstanceId() != target.getInstanceId())
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (target is Merchant merchant1)
			{
				merchant = merchant1;
			}
			else
			{
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}
		}
		
		if (merchant == null && !player.isGM() && _listId != CUSTOM_CB_SELL_LIST)
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
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

		if (merchant != null && !buyList.isNpcAllowed(merchant.getId()))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		long totalPrice = 0;
		// Proceed the sell
		foreach (UniqueItemHolder i in _items)
		{
			Item item = player.checkItemManipulation(i.ObjectId, i.getCount(), "sell");
			if (item == null || !item.isSellable())
			{
				continue;
			}
			
			long price = item.getReferencePrice() / 2;
			totalPrice += price * i.getCount();
			if (Inventory.MAX_ADENA / i.getCount() < price || totalPrice > Inventory.MAX_ADENA)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to purchase over " + Inventory.MAX_ADENA + " adena worth of goods.", Config.DEFAULT_PUNISH);
				
				return ValueTask.CompletedTask;
			}

			if (Config.ALLOW_REFUND)
			{
				player.getInventory().transferItem("Sell", i.ObjectId, i.getCount(), player.getRefund(), player, merchant);
			}
			else
			{
				player.getInventory().destroyItem("Sell", i.ObjectId, i.getCount(), player, merchant);
			}
		}
		
		if (!Config.MERCHANT_ZERO_SELL_PRICE)
		{
			player.addAdena("Sell", totalPrice, merchant, false);
		}
		
		// Update current load as well
		player.sendPacket(new ExUserInfoInventoryWeightPacket(player));
		player.sendPacket(new ExBuySellListPacket(player, true));
		return ValueTask.CompletedTask;
	}
}