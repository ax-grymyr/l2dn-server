using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.PrimeShop;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;

public readonly struct ExBRProductListPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly int _type;
	private readonly ICollection<PrimeShopGroup> _primeList;

	public ExBRProductListPacket(Player player, int type, ICollection<PrimeShopGroup> items)
	{
		_player = player;
		_type = type;
		_primeList = items;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_BR_PRODUCT_LIST);

		writer.WriteInt64(_player.getAdena()); // Adena
		writer.WriteInt64(0); // Hero coins
		writer.WriteByte((byte)_type); // Type 0 - Home, 1 - History, 2 - Favorites
		writer.WriteInt32(_primeList.Count);
		foreach (PrimeShopGroup brItem in _primeList)
		{
			writer.WriteInt32(brItem.getBrId());
			writer.WriteByte((byte)brItem.getCat());
			writer.WriteByte((byte)brItem.getPaymentType()); // Payment Type: 0 - Prime Points, 1 - Adena, 2 - Hero Coins
			writer.WriteInt32(brItem.getPrice());
			writer.WriteByte((byte)brItem.getPanelType()); // Item Panel Type: 0 - None, 1 - Event, 2 - Sale, 3 - New, 4 - Best
			writer.WriteInt32(brItem.getRecommended()); // Recommended: (bit flags) 1 - Top, 2 - Left, 4 - Right
			writer.WriteInt32(brItem.getStartSale()?.getEpochSecond() ?? 0);
			writer.WriteInt32(brItem.getEndSale()?.getEpochSecond() ?? 0);
			writer.WriteByte((byte)brItem.getDaysOfWeek());
			writer.WriteByte((byte)brItem.getStartHour());
			writer.WriteByte((byte)brItem.getStartMinute());
			writer.WriteByte((byte)brItem.getStopHour());
			writer.WriteByte((byte)brItem.getStopMinute());

			// Daily account limit.
			if (brItem.getAccountDailyLimit() > 0 && _player.getAccountVariables().Get(AccountVariables.PRIME_SHOP_PRODUCT_DAILY_COUNT + brItem.getBrId(), 0) >= brItem.getAccountDailyLimit())
			{
				writer.WriteInt32(brItem.getAccountDailyLimit());
				writer.WriteInt32(brItem.getAccountDailyLimit());
			}
			// General account limit.
			else if (brItem.getAccountBuyLimit() > 0 && _player.getAccountVariables().Get(AccountVariables.PRIME_SHOP_PRODUCT_COUNT + brItem.getBrId(), 0) >= brItem.getAccountBuyLimit())
			{
				writer.WriteInt32(brItem.getAccountBuyLimit());
				writer.WriteInt32(brItem.getAccountBuyLimit());
			}
			else
			{
				writer.WriteInt32(brItem.getStock());
				writer.WriteInt32(brItem.getTotal());
			}

			writer.WriteByte((byte)brItem.getSalePercent());
			writer.WriteByte((byte)brItem.getMinLevel());
			writer.WriteByte((byte)brItem.getMaxLevel());
			writer.WriteInt32(brItem.getMinBirthday());
			writer.WriteInt32(brItem.getMaxBirthday());

			// Daily account limit.
			if (brItem.getAccountDailyLimit() > 0)
			{
				writer.WriteInt32(1); // Days
				writer.WriteInt32(brItem.getAccountDailyLimit()); // Amount
			}
			// General account limit.
			else if (brItem.getAccountBuyLimit() > 0)
			{
				writer.WriteInt32(-1); // Days
				writer.WriteInt32(brItem.getAccountBuyLimit()); // Amount
			}
			else
			{
				writer.WriteInt32(0); // Days
				writer.WriteInt32(0); // Amount
			}

			writer.WriteByte((byte)brItem.getItems().Count);
			foreach (PrimeShopItem item in brItem.getItems())
			{
				writer.WriteInt32(item.getId());
				writer.WriteInt32((int) item.getCount());
				writer.WriteInt32(item.getWeight());
				writer.WriteInt32(item.isTradable());
			}
		}
	}
}