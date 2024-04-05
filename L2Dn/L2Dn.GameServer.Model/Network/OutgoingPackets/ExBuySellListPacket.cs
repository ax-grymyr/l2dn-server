using System.Collections.Immutable;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct ExBuySellListPacket: IOutgoingPacket
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ExBuySellListPacket));

	public const int BUY_SELL_LIST_BUY = 0; // TODO enums
	public const int BUY_SELL_LIST_SELL = 1;
	public const int BUY_SELL_LIST_UNK = 2;
	public const int BUY_SELL_LIST_TAX = 3;

	public const int UNK_SELECT_FIRST_TAB = 0;
	public const int UNK_SHOW_PURCHASE_LIST = 1;
	public const int UNK_SEND_NOT_ENOUGH_ADENA_MESSAGE = 2;
	public const int UNK_SEND_INCORRECT_ITEM_MESSAGE = 3;

	private static readonly int[] CASTLES =
	{
		3, // Giran
		7, // Goddart
		5, // Aden
	};

	private readonly int _inventorySlots;
	private readonly int _type;

	// buy type - BUY
	private long _money;
	private double _castleTaxRate;
	private ImmutableArray<Product> _list;
	private int _listId;

	// buy type - SELL
	private readonly List<Item> _sellList;
	private readonly List<Item> _refundList;
	private bool _done;

	// buy type = unk
	private int _unkType;

	// buy type - send tax
	private bool _applyTax;

	public ExBuySellListPacket(ProductList list, Player player, double castleTaxRate)
	{
		_type = BUY_SELL_LIST_BUY;
		_listId = list.getListId();
		_list = list.getProducts();
		_money = player.isGM() && player.getAdena() == 0 && list.getNpcsAllowed().IsEmpty
			? 1000000000
			: player.getAdena();
		_inventorySlots = player.getInventory().getNonQuestSize();
		_castleTaxRate = castleTaxRate;
	}

	public ExBuySellListPacket(Player player, bool done)
	{
		_type = BUY_SELL_LIST_SELL;
		_sellList = [];
		_refundList = [];
		Summon pet = player.getPet();
		foreach (Item item in player.getInventory().getItems())
		{
			if (!item.isEquipped() && item.isSellable() &&
			    (pet == null || item.getObjectId() != pet.getControlObjectId()))
				_sellList.Add(item);
		}

		_inventorySlots = player.getInventory().getNonQuestSize();
		if (player.hasRefund())
			_refundList.AddRange(player.getRefund().getItems());

		_done = done;
	}

	public ExBuySellListPacket(int type)
	{
		_type = BUY_SELL_LIST_UNK;
		_unkType = type;
		_inventorySlots = 0;
	}

	public ExBuySellListPacket(bool fakeParam)
	{
		_type = BUY_SELL_LIST_TAX;
		_inventorySlots = 0;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_BUY_SELL_LIST);
		writer.WriteInt32(_type);
		switch (_type)
		{
			case BUY_SELL_LIST_BUY:
			{
				sendBuyList(writer);
				break;
			}
			case BUY_SELL_LIST_SELL:
			{
				sendSellList(writer);
				break;
			}
			case BUY_SELL_LIST_UNK:
			{
				sendUnk(writer);
				break;
			}
			case BUY_SELL_LIST_TAX:
			{
				sendCurrentTax(writer);
				break;
			}
			default:
			{
				_logger.Error(GetType().Name + ": unknown type " + _type);
				break;
			}
		}
	}

	private void sendBuyList(PacketBitWriter writer)
	{
		writer.WriteInt64(_money); // current money
		writer.WriteInt32(_listId);
		writer.WriteInt32(_inventorySlots);
		writer.WriteInt16((short)_list.Length);
		foreach (Product product in _list)
		{
			if (product.getCount() > 0 || !product.hasLimitedStock())
			{
				InventoryPacketHelper.WriteItem(writer, new ItemInfo(product));
				writer.WriteInt64((long)(product.getPrice() * (1.0 + _castleTaxRate + product.getBaseTaxRate())));
			}
		}
	}

	private void sendSellList(PacketBitWriter writer)
	{
		writer.WriteInt32(_inventorySlots);
		if (!_sellList.isEmpty())
		{
			writer.WriteInt16((short)_sellList.size());
			foreach (Item item in _sellList)
			{
				InventoryPacketHelper.WriteItem(writer, new ItemInfo(item));
				writer.WriteInt64(Config.MERCHANT_ZERO_SELL_PRICE ? 0 : item.getTemplate().getReferencePrice() / 2);
			}
		}
		else
		{
			writer.WriteInt16(0);
		}

		if (!_refundList.isEmpty())
		{
			writer.WriteInt16((short)_refundList.size());
			int i = 0;
			foreach (Item item in _refundList)
			{
				InventoryPacketHelper.WriteItem(writer, new ItemInfo(item));
				writer.WriteInt32(i++);
				writer.WriteInt64(Config.MERCHANT_ZERO_SELL_PRICE
					? 0
					: item.getTemplate().getReferencePrice() / 2 * item.getCount());
			}
		}
		else
		{
			writer.WriteInt16(0);
		}

		writer.WriteByte(_done);
	}

	private void sendUnk(PacketBitWriter writer)
	{
		writer.WriteByte((byte)_unkType);
	}

	private void sendCurrentTax(PacketBitWriter writer)
	{
		writer.WriteInt32(CASTLES.Length);
		foreach (int id in CASTLES)
		{
			writer.WriteInt32(id); // residence id
			try
			{
				writer.WriteInt32(_applyTax
					? CastleManager.getInstance().getCastleById(id).getTaxPercent(TaxType.BUY)
					: 0); // residence tax
			}
			catch (NullReferenceException e)
			{
				_logger.Error(e);
				writer.WriteInt32(0);
			}
		}
	}
}