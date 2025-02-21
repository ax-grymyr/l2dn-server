using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct SetPrivateStoreListBuyPacket: IIncomingPacket<GameSession>
{
    private TradeItem[]? _items;

    public void ReadContent(PacketBitReader reader)
    {
		int count = reader.ReadInt32();
		if (count < 1 || count > Config.MAX_ITEM_IN_PACKET)
			return;

		_items = new TradeItem[count];
		for (int i = 0; i < count; i++)
		{
			int itemId = reader.ReadInt32();
			ItemTemplate? template = ItemData.getInstance().getTemplate(itemId);
			if (template == null)
			{
				_items = null;
				return;
			}

			int enchantLevel = reader.ReadInt16();
			reader.ReadInt16(); // TODO analyse this

			long cnt = reader.ReadInt64();
			long price = reader.ReadInt64();
			if (itemId < 1 || cnt < 1 || price < 0)
			{
				_items = null;
				return;
			}

			int option1 = reader.ReadInt32();
			int option2 = reader.ReadInt32();
			AttributeType attackAttribute = (AttributeType)reader.ReadInt16();
			int attackAttributeValue = reader.ReadInt16();
			int defenceFire = reader.ReadInt16();
			int defenceWater = reader.ReadInt16();
			int defenceWind = reader.ReadInt16();
			int defenceEarth = reader.ReadInt16();
			int defenceHoly = reader.ReadInt16();
			int defenceDark = reader.ReadInt16();
			int visualId = reader.ReadInt32();

			EnsoulOption[] soulCrystalOptions = new EnsoulOption[reader.ReadByte()];
            for (int k = 0; k < soulCrystalOptions.Length; k++)
            {
                EnsoulOption? option = EnsoulData.getInstance().getOption(reader.ReadInt32());
                if (option != null)
                    soulCrystalOptions[k] = option;
            }

            EnsoulOption[] soulCrystalSpecialOptions = new EnsoulOption[reader.ReadByte()];
            for (int k = 0; k < soulCrystalSpecialOptions.Length; k++)
            {
                EnsoulOption? option = EnsoulData.getInstance().getOption(reader.ReadInt32());
                if (option != null)
                    soulCrystalSpecialOptions[k] = option;
            }

			// Unknown.
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadString();

			TradeItem item = new TradeItem(template, cnt, price);
			item.setEnchant(enchantLevel);
			item.setAugmentation(option1, option2);
			item.setAttackElementType(attackAttribute);
			item.setAttackElementPower(attackAttributeValue);
			item.setElementDefAttr(AttributeType.FIRE, defenceFire);
			item.setElementDefAttr(AttributeType.WATER, defenceWater);
			item.setElementDefAttr(AttributeType.WIND, defenceWind);
			item.setElementDefAttr(AttributeType.EARTH, defenceEarth);
			item.setElementDefAttr(AttributeType.HOLY, defenceHoly);
			item.setElementDefAttr(AttributeType.DARK, defenceDark);
			item.setVisualId(visualId);
			item.setSoulCrystalOptions(soulCrystalOptions);
			item.setSoulCrystalSpecialOptions(soulCrystalSpecialOptions);
			_items[i] = item;
		}
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

		if (_items == null)
		{
			player.setPrivateStoreType(PrivateStoreType.NONE);
			player.broadcastUserInfo();
			return ValueTask.CompletedTask;
		}

		if (!player.getAccessLevel().allowTransaction())
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return ValueTask.CompletedTask;
		}

		if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(player) || player.isInDuel())
		{
			player.sendPacket(SystemMessageId.WHILE_YOU_ARE_ENGAGED_IN_COMBAT_YOU_CANNOT_OPERATE_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
			player.sendPacket(new PrivateStoreManageListBuyPacket(1, player));
			player.sendPacket(new PrivateStoreManageListBuyPacket(2, player));
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (player.isInsideZone(ZoneId.NO_STORE))
		{
			player.sendPacket(new PrivateStoreManageListBuyPacket(1, player));
			player.sendPacket(new PrivateStoreManageListBuyPacket(2, player));
			player.sendPacket(SystemMessageId.YOU_CANNOT_OPEN_A_PRIVATE_STORE_HERE);
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		TradeList tradeList = player.getBuyList();
		tradeList.clear();

		// Check maximum number of allowed slots for pvt shops
		if (_items.Length > player.getPrivateBuyStoreLimit())
		{
			player.sendPacket(new PrivateStoreManageListBuyPacket(1, player));
			player.sendPacket(new PrivateStoreManageListBuyPacket(2, player));
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
			return ValueTask.CompletedTask;
		}

		long totalCost = 0;
		foreach (TradeItem i in _items)
		{
			if (Inventory.MAX_ADENA / i.getCount() < i.getPrice())
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to set price more than " + Inventory.MAX_ADENA + " adena in Private Store - Buy.",
					Config.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}

			tradeList.addItemByItemId(i.getItem().getId(), i.getCount(), i.getPrice());
			totalCost += i.getCount() * i.getPrice();
			if (totalCost > Inventory.MAX_ADENA)
			{
				Util.handleIllegalPlayerAction(player,
					"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
					" tried to set total price more than " + Inventory.MAX_ADENA + " adena in Private Store - Buy.",
					Config.DEFAULT_PUNISH);

				return ValueTask.CompletedTask;
			}
		}

		// Check for available funds
		if (totalCost > player.getAdena())
		{
			player.sendPacket(new PrivateStoreManageListBuyPacket(1, player));
			player.sendPacket(new PrivateStoreManageListBuyPacket(2, player));
			player.sendPacket(SystemMessageId.THE_PURCHASE_PRICE_IS_HIGHER_THAN_THE_AMOUNT_OF_MONEY_THAT_YOU_HAVE_AND_SO_YOU_CANNOT_OPEN_A_PERSONAL_STORE);
			return ValueTask.CompletedTask;
		}

		player.sitDown();
		player.setPrivateStoreType(PrivateStoreType.BUY);
		player.broadcastUserInfo();
		player.broadcastPacket(new PrivateStoreMsgBuyPacket(player));

		return ValueTask.CompletedTask;
    }
}