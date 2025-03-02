using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.LimitShop;
using L2Dn.GameServer.Network.OutgoingPackets.PrimeShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.LimitShop;

public struct RequestPurchaseLimitShopItemBuyPacket: IIncomingPacket<GameSession>
{
    private int _shopIndex;
    private int _productId;
    private int _amount;
    private LimitShopProductHolder? _product;

    public void ReadContent(PacketBitReader reader)
    {
        _shopIndex = reader.ReadByte(); // 3 Lcoin Store, 4 Special Craft, 100 Clan Shop
        _productId = reader.ReadInt32();
        _amount = reader.ReadInt32();

        switch (_shopIndex)
        {
            case 3: // Normal Lcoin Shop
            {
                _product = LimitShopData.getInstance().getProduct(_productId);
                break;
            }
            case 4: // Lcoin Special Craft
            {
                _product = LimitShopCraftData.getInstance().getProduct(_productId);
                break;
            }
            case 100: // Clan Shop
            {
                _product = LimitShopClanData.getInstance().getProduct(_productId);
                break;
            }
            default:
            {
                _product = null;
                break;
            }
        }

        reader.ReadInt32(); // SuccessionItemSID
        reader.ReadInt32(); // MaterialItemSID
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		if (_product == null)
			return ValueTask.CompletedTask;

		if (_amount < 1 || _amount > 10000)
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVENTORY_OVERFLOW));
			player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
				new List<LimitShopRandomCraftReward>()));

			return ValueTask.CompletedTask;
		}

		if (!player.isInventoryUnder80(false))
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVENTORY_OVERFLOW));
			player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
				new List<LimitShopRandomCraftReward>()));

			return ValueTask.CompletedTask;
		}

		if (player.getLevel() < _product.getMinLevel() || player.getLevel() > _product.getMaxLevel())
		{
			player.sendPacket(SystemMessageId.YOUR_LEVEL_CANNOT_PURCHASE_THIS_ITEM);
			player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
				new List<LimitShopRandomCraftReward>()));

			return ValueTask.CompletedTask;
		}

		if (player.hasItemRequest() || player.hasRequest<PrimeShopRequest>())
		{
			player.sendPacket(new ExBRBuyProductPacket(ExBrProductReplyType.INVALID_USER_STATE));
			player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
				new List<LimitShopRandomCraftReward>()));

			return ValueTask.CompletedTask;
		}

		// Add request.
		player.addRequest(new PrimeShopRequest(player));

		// Check limits.
		if (_product.getAccountDailyLimit() > 0) // Sale period.
		{
			long amount = _product.getAccountDailyLimit() * _amount;
			if (amount < 1)
			{
				player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
				player.removeRequest<PrimeShopRequest>();
				player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
					new List<LimitShopRandomCraftReward>()));

				return ValueTask.CompletedTask;
			}

			if (player.getAccountVariables()
				    .Get(AccountVariables.LCOIN_SHOP_PRODUCT_DAILY_COUNT + _product.getProductionId(), 0) >= amount)
			{
				player.sendMessage("You have reached your daily limit."); // TODO: Retail system message?
				player.removeRequest<PrimeShopRequest>();
				player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
					new List<LimitShopRandomCraftReward>()));

				return ValueTask.CompletedTask;
			}
		}
		else if (_product.getAccountMontlyLimit() > 0)
		{
			long amount = _product.getAccountMontlyLimit() * _amount;
			if (amount < 1)
			{
				player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
				player.removeRequest<PrimeShopRequest>();
				player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
					new List<LimitShopRandomCraftReward>()));

				return ValueTask.CompletedTask;
			}
			if (player.getAccountVariables().Get(AccountVariables.LCOIN_SHOP_PRODUCT_MONTLY_COUNT + _product.getProductionId(), 0) >= amount)
			{
				player.sendMessage("You have reached your montly limit.");
				player.removeRequest<PrimeShopRequest>();
				player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
					new List<LimitShopRandomCraftReward>()));

				return ValueTask.CompletedTask;
			}

		}
		else if (_product.getAccountBuyLimit() > 0) // Count limit.
		{
			long amount = _product.getAccountBuyLimit() * _amount;
			if (amount < 1)
			{
				player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
				player.removeRequest<PrimeShopRequest>();
				player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
					new List<LimitShopRandomCraftReward>()));

				return ValueTask.CompletedTask;
			}

			if (player.getAccountVariables().Get(AccountVariables.LCOIN_SHOP_PRODUCT_COUNT + _product.getProductionId(), 0) >= amount)
			{
				player.sendMessage("You cannot buy any more of this item."); // TODO: Retail system message?
				player.removeRequest<PrimeShopRequest>();
				player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, 0,
					new List<LimitShopRandomCraftReward>()));

				return ValueTask.CompletedTask;
			}
		}

		// Check existing items.
		int remainingInfo = Math.Max(0,
			Math.Max(_product.getAccountBuyLimit(),
				Math.Max(_product.getAccountDailyLimit(), _product.getAccountMontlyLimit())));

		for (int i = 0; i < _product.getIngredientIds().Length; i++)
		{
			if (_product.getIngredientIds()[i] == 0)
			{
				continue;
			}
			if (_product.getIngredientIds()[i] == Inventory.ADENA_ID)
			{
				long amount = _product.getIngredientQuantities()[i] * _amount;
				if (amount < 1)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}

				if (player.getAdena() < amount)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}
			}
			else if (_product.getIngredientIds()[i] == (int)SpecialItemType.HONOR_COINS)
			{
				long amount = _product.getIngredientQuantities()[i] * _amount;
				if (amount < 1)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}

				if (player.getHonorCoins() < amount)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}
			}
			else if (_product.getIngredientIds()[i] == (int)SpecialItemType.PC_CAFE_POINTS)
			{
				long amount = _product.getIngredientQuantities()[i] * _amount;
				if (amount < 1)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}

				if (player.getPcCafePoints() < amount)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}
			}
			else
			{
				long amount = _product.getIngredientQuantities()[i] * _amount;
				if (amount < 1)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}

				if (player.getInventory().getInventoryItemCount(_product.getIngredientIds()[i],
					    _product.getIngredientEnchants()[i] == 0 ? -1 : _product.getIngredientEnchants()[i], true) <
				    amount)
				{
					player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
					player.removeRequest<PrimeShopRequest>();
					player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId,
						remainingInfo, new List<LimitShopRandomCraftReward>()));

					return ValueTask.CompletedTask;
				}
			}
		}

		// Remove items.
		for (int i = 0; i < _product.getIngredientIds().Length; i++)
		{
			if (_product.getIngredientIds()[i] == 0)
			{
				continue;
			}
			if (_product.getIngredientIds()[i] == Inventory.ADENA_ID)
			{
				player.reduceAdena("LCoinShop", _product.getIngredientQuantities()[i] * _amount, player, true);
			}
			else if (_product.getIngredientIds()[i] == (int)SpecialItemType.HONOR_COINS)
			{
				player.setHonorCoins(player.getHonorCoins() - _product.getIngredientQuantities()[i] * _amount);
			}
			else if (_product.getIngredientIds()[i] == (int)SpecialItemType.PC_CAFE_POINTS)
			{
				int newPoints = (int) (player.getPcCafePoints() - _product.getIngredientQuantities()[i] * _amount);
				player.setPcCafePoints(newPoints);
				player.sendPacket(new ExPcCafePointInfoPacket(player.getPcCafePoints(), (int) -(_product.getIngredientQuantities()[i] * _amount), 1));
			}
			else
			{
				if (_product.getIngredientEnchants()[i] > 0)
				{
					int count = 0;
					ICollection<Item> items = player.getInventory().getAllItemsByItemId(_product.getIngredientIds()[i], _product.getIngredientEnchants()[i]);
					foreach (Item item in items)
					{
						if (count == _amount)
						{
							break;
						}
						count++;
						player.destroyItem("LCoinShop", item, player, true);
					}
				}
				else
				{
					long amount = _product.getIngredientQuantities()[i] * _amount;
					if (amount < 1)
					{
						player.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
						player.removeRequest<PrimeShopRequest>();
						player.sendPacket(new ExPurchaseLimitShopItemResultPacket(false, _shopIndex, _productId, remainingInfo, new List<LimitShopRandomCraftReward>()));
						return ValueTask.CompletedTask;
					}

					player.destroyItemByItemId("LCoinShop", _product.getIngredientIds()[i], amount, player, true);
				}
			}

			if (Config.VIP_SYSTEM_L_SHOP_AFFECT)
			{
				player.updateVipPoints(_amount);
			}
		}

		// Reward.
		Map<int, LimitShopRandomCraftReward> rewards = new();
		if (_product.getProductionId2() > 0)
		{
			LimitShopProductHolder product = _product;
			for (int i = 0; i < _amount; i++)
			{
				if (Rnd.get(100) < _product.getChance())
				{
					rewards.GetOrAdd(0, _ => new LimitShopRandomCraftReward(product.getProductionId(), 0, 0))
						.Count += (int)_product.getCount();

					Item? item = player.addItem("LCoinShop", _product.getProductionId(), _product.getCount(),
						_product.getEnchant(), player, true);

                    if (item == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: atomic inventory update
                        return ValueTask.CompletedTask;
                    }

					if (_product.isAnnounce())
					{
						Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item,
							ExItemAnnouncePacket.SPECIAL_CREATION));
					}
				}
				else if (Rnd.get(100) < _product.getChance2() || _product.getProductionId3() == 0)
				{
					rewards.GetOrAdd(1, _ => new LimitShopRandomCraftReward(product.getProductionId2(), 0, 1))
						.Count += (int)_product.getCount2();

					Item? item = player.addItem("LCoinShop", _product.getProductionId2(), _product.getCount2(), player,
						true);

                    if (item == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: atomic inventory update
                        return ValueTask.CompletedTask;
                    }

					if (_product.isAnnounce2())
					{
						Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item,
							ExItemAnnouncePacket.SPECIAL_CREATION));
					}
				}
				else if (Rnd.get(100) < _product.getChance3() || _product.getProductionId4() == 0)
				{
					rewards.GetOrAdd(2, _ => new LimitShopRandomCraftReward(product.getProductionId3(), 0, 2))
						.Count += (int)_product.getCount3();

					Item? item = player.addItem("LCoinShop", _product.getProductionId3(), _product.getCount3(), player,
						true);

                    if (item == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: atomic inventory update
                        return ValueTask.CompletedTask;
                    }

					if (_product.isAnnounce3())
					{
						Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item,
							ExItemAnnouncePacket.SPECIAL_CREATION));
					}
				}
				else if (Rnd.get(100) < _product.getChance4() || _product.getProductionId5() == 0)
				{
					rewards.GetOrAdd(3, _ => new LimitShopRandomCraftReward(product.getProductionId4(), 0, 3))
						.Count += (int)_product.getCount4();

					Item? item = player.addItem("LCoinShop", _product.getProductionId4(), _product.getCount4(), player,
						true);

                    if (item == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: atomic inventory update
                        return ValueTask.CompletedTask;
                    }

					if (_product.isAnnounce4())
					{
						Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item,
							ExItemAnnouncePacket.SPECIAL_CREATION));
					}
				}
				else if (_product.getProductionId5() > 0)
				{
					rewards.GetOrAdd(4, _ => new LimitShopRandomCraftReward(product.getProductionId5(), 0, 4))
						.Count += (int)_product.getCount5();

					Item? item = player.addItem("LCoinShop", _product.getProductionId5(), _product.getCount5(), player,
						true);

                    if (item == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: atomic inventory update
                        return ValueTask.CompletedTask;
                    }

					if (_product.isAnnounce5())
					{
						Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item,
							ExItemAnnouncePacket.SPECIAL_CREATION));
					}
				}
			}
		}
		else if (Rnd.get(100) < _product.getChance())
		{
			rewards.put(0,
				new LimitShopRandomCraftReward(_product.getProductionId(), (int)(_product.getCount() * _amount), 0));

			Item? item = player.addItem("LCoinShop", _product.getProductionId(), _product.getCount() * _amount,
				_product.getEnchant(), player, true);

            if (item == null)
            {
                player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL); // TODO: atomic inventory update
                return ValueTask.CompletedTask;
            }

			if (_product.isAnnounce())
			{
				Broadcast.toAllOnlinePlayers(new ExItemAnnouncePacket(player, item,
					ExItemAnnouncePacket.SPECIAL_CREATION));
			}
		}

		// Update account variables.
		if (_product.getAccountDailyLimit() > 0)
		{
			player.getAccountVariables()
				.Set(AccountVariables.LCOIN_SHOP_PRODUCT_DAILY_COUNT + _product.getProductionId(),
					player.getAccountVariables()
						.Get(AccountVariables.LCOIN_SHOP_PRODUCT_DAILY_COUNT + _product.getProductionId(), 0) +
					_amount);
		}

		if (_product.getAccountMontlyLimit() > 0)
		{
			player.getAccountVariables()
				.Set(AccountVariables.LCOIN_SHOP_PRODUCT_MONTLY_COUNT + _product.getProductionId(),
					player.getAccountVariables()
						.Get(AccountVariables.LCOIN_SHOP_PRODUCT_MONTLY_COUNT + _product.getProductionId(), 0) +
					_amount);
		}
		else if (_product.getAccountBuyLimit() > 0)
		{
			player.getAccountVariables().Set(AccountVariables.LCOIN_SHOP_PRODUCT_COUNT + _product.getProductionId(),
				player.getAccountVariables()
					.Get(AccountVariables.LCOIN_SHOP_PRODUCT_COUNT + _product.getProductionId(), 0) + _amount);
		}

		player.sendPacket(new ExPurchaseLimitShopItemResultPacket(true, _shopIndex, _productId,
			Math.Max(remainingInfo - _amount, 0), rewards.Values));

		player.sendItemList();

		// Remove request.
		player.removeRequest<PrimeShopRequest>();

        return ValueTask.CompletedTask;
    }
}