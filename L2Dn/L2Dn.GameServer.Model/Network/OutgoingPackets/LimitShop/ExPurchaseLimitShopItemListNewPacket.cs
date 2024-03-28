using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.LimitShop;

public readonly struct ExPurchaseLimitShopItemListNewPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly int _shopType; // 3 Lcoin Store, 4 Special Craft, 100 Clan Shop
	private readonly int _page;
	private readonly int _totalPages;
	private readonly ICollection<LimitShopProductHolder> _products;
	
	public ExPurchaseLimitShopItemListNewPacket(Player player, int shopType, int page, int totalPages, ICollection<LimitShopProductHolder> products)
	{
		_player = player;
		_shopType = shopType;
		_page = page;
		_totalPages = totalPages;
		_products = products;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_PURCHASE_LIMIT_SHOP_ITEM_LIST_NEW);
		
		writer.WriteByte((byte)_shopType);
		writer.WriteByte((byte)_page); // 311
		writer.WriteByte((byte)_totalPages); // 311
		writer.WriteInt32(_products.Count);
		foreach (LimitShopProductHolder product in _products)
		{
			writer.WriteInt32(product.getId());
			writer.WriteInt32(product.getProductionId());
			writer.WriteInt32(product.getIngredientIds()[0]);
			writer.WriteInt32(product.getIngredientIds()[1]);
			writer.WriteInt32(product.getIngredientIds()[2]);
			writer.WriteInt32(product.getIngredientIds()[3]); // 306
			writer.WriteInt32(product.getIngredientIds()[4]); // 306
			writer.WriteInt64(product.getIngredientQuantities()[0]);
			writer.WriteInt64(product.getIngredientQuantities()[1]);
			writer.WriteInt64(product.getIngredientQuantities()[2]);
			writer.WriteInt64(product.getIngredientQuantities()[3]); // 306
			writer.WriteInt64(product.getIngredientQuantities()[4]); // 306
			writer.WriteInt16((short)product.getIngredientEnchants()[0]);
			writer.WriteInt16((short)product.getIngredientEnchants()[1]);
			writer.WriteInt16((short)product.getIngredientEnchants()[2]);
			writer.WriteInt16((short)product.getIngredientEnchants()[3]); // 306
			writer.WriteInt16((short)product.getIngredientEnchants()[4]); // 306
			// Check limits.
			if (product.getAccountDailyLimit() > 0) // Sale period.
			{
				if (_player.getAccountVariables().getInt(AccountVariables.LCOIN_SHOP_PRODUCT_DAILY_COUNT + product.getProductionId(), 0) >= product.getAccountDailyLimit())
				{
					writer.WriteInt32(0);
				}
				else
				{
					writer.WriteInt32(product.getAccountDailyLimit() - _player.getAccountVariables().getInt(AccountVariables.LCOIN_SHOP_PRODUCT_DAILY_COUNT + product.getProductionId(), 0));
				}
			}
			else if (product.getAccountMontlyLimit() > 0)
			{
				if (_player.getAccountVariables().getInt(AccountVariables.LCOIN_SHOP_PRODUCT_MONTLY_COUNT + product.getProductionId(), 0) >= product.getAccountMontlyLimit())
				{
					writer.WriteInt32(0);
				}
				else
				{
					writer.WriteInt32(product.getAccountMontlyLimit() - _player.getAccountVariables().getInt(AccountVariables.LCOIN_SHOP_PRODUCT_MONTLY_COUNT + product.getProductionId(), 0));
				}
			}
			else if (product.getAccountBuyLimit() > 0) // Count limit.
			{
				if (_player.getAccountVariables().getInt(AccountVariables.LCOIN_SHOP_PRODUCT_COUNT + product.getProductionId(), 0) >= product.getAccountBuyLimit())
				{
					writer.WriteInt32(0);
				}
				else
				{
					writer.WriteInt32(product.getAccountBuyLimit() - _player.getAccountVariables().getInt(AccountVariables.LCOIN_SHOP_PRODUCT_COUNT + product.getProductionId(), 0));
				}
			}
			else // No account limits.
			{
				writer.WriteInt32(1);
			}
			writer.WriteInt32(0); // nRemainSec
			writer.WriteInt32(0); // nRemainServerItemAmount
			writer.WriteInt16(0); // sCircleNum (311)
		}
	}
}