using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct MultiSellChoosePacket: IIncomingPacket<GameSession>
{
	private int _listId;
	private int _entryId;
	private long _amount;
	private int _enchantLevel;
	private int _augmentOption1;
	private int _augmentOption2;
	private AttributeType _attackAttribute;
	private short _attributePower;
	private short _fireDefence;
	private short _waterDefence;
	private short _windDefence;
	private short _earthDefence;
	private short _holyDefence;
	private short _darkDefence;
	private EnsoulOption[] _soulCrystalOptions;
	private EnsoulOption[] _soulCrystalSpecialOptions;

	public void ReadContent(PacketBitReader reader)
	{
		_listId = reader.ReadInt32();
		_entryId = reader.ReadInt32();
		_amount = reader.ReadInt64();
		_enchantLevel = reader.ReadInt16();
		_augmentOption1 = reader.ReadInt32();
		_augmentOption2 = reader.ReadInt32();
		_attackAttribute = (AttributeType)reader.ReadInt16();
		_attributePower = reader.ReadInt16();
		_fireDefence = reader.ReadInt16();
		_waterDefence = reader.ReadInt16();
		_windDefence = reader.ReadInt16();
		_earthDefence = reader.ReadInt16();
		_holyDefence = reader.ReadInt16();
		_darkDefence = reader.ReadInt16();

		_soulCrystalOptions = new EnsoulOption[reader.ReadByte()]; // Ensoul size
		for (int i = 0; i < _soulCrystalOptions.Length; i++)
		{
			int ensoulId = reader.ReadInt32(); // Ensoul option id
            EnsoulOption? option = EnsoulData.getInstance().getOption(ensoulId);
            if (option != null)
			    _soulCrystalOptions[i] = option;
		}

		_soulCrystalSpecialOptions = new EnsoulOption[reader.ReadByte()]; // Special ensoul size
		for (int i = 0; i < _soulCrystalSpecialOptions.Length; i++)
		{
			int ensoulId = reader.ReadInt32(); // Special ensoul option id.
            EnsoulOption? option = EnsoulData.getInstance().getOption(ensoulId);
            if (option != null)
                _soulCrystalSpecialOptions[i] = option;
		}
	}

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player is null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canUseMultiSell())
        // {
        // 	player.setMultiSell(null);
        // 	return ValueTask.CompletedTask;
        // }

        if (_amount < 1 || _amount > 10000) // 999 999 is client max.
        {
            player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
            return ValueTask.CompletedTask;
        }

        PreparedMultisellListHolder? list = player.getMultiSell();
        if (list == null || list.Id != _listId)
        {
            player.setMultiSell(null);
            return ValueTask.CompletedTask;
        }

        Npc? npc = player.getLastFolkNPC();
        if (!list.isNpcAllowed(-1))
        {
            if (npc == null //
                || !list.isNpcAllowed(npc.Id) //
                || !list.checkNpcObjectId(npc.ObjectId) //
                || player.getInstanceId() != npc.getInstanceId() //
                || !player.IsInsideRadius3D(npc, Npc.INTERACTION_DISTANCE))
            {
                if (player.isGM())
                {
                    player.sendMessage("Multisell " + _listId +
                        " is restricted. Under current conditions cannot be used. Only GMs are allowed to use it.");
                }
                else
                {
                    player.setMultiSell(null);
                    return ValueTask.CompletedTask;
                }
            }
        }

        if (_soulCrystalOptions != null && _soulCrystalOptions.ContainsNull() ||
            (_soulCrystalSpecialOptions != null && _soulCrystalSpecialOptions.ContainsNull()))
        {
            PacketLogger.Instance.Warn("Character: " + player.getName() +
                " requested multisell entry with invalid soul crystal options. Multisell: " +
                _listId + " entry: " + _entryId);

            player.setMultiSell(null);
            return ValueTask.CompletedTask;
        }

        ImmutableArray<MultisellEntryHolder> entries = list.getEntries();
        if (entries.IsDefaultOrEmpty)
        {
            PacketLogger.Instance.Warn("Character: " + player.getName() +
                " requested empty multisell entry. Multisell: " + _listId + " entry: " +
                _entryId);

            return ValueTask.CompletedTask;
        }

        if (entries.IsDefaultOrEmpty)
        {
            PacketLogger.Instance.Warn("Character: " + player.getName() +
                " requested empty multisell entry. Multisell: " + _listId + " entry: " +
                _entryId);

            return ValueTask.CompletedTask;
        }

        if (_entryId < 0 || _entryId >= entries.Length)
        {
            PacketLogger.Instance.Warn("Character: " + player.getName() +
                " requested out of bounds multisell entry. Multisell: " + _listId + " entry: " +
                _entryId);

            return ValueTask.CompletedTask;
        }

        MultisellEntryHolder entry = entries[_entryId];
        if (entry == null)
        {
            PacketLogger.Instance.Warn("Character: " + player.getName() +
                " requested inexistant prepared multisell entry. Multisell: " + _listId +
                " entry: " + _entryId);

            player.setMultiSell(null);
            return ValueTask.CompletedTask;
        }

        if (!entry.isStackable() && _amount > 1)
        {
            PacketLogger.Instance.Warn("Character: " + player.getName() +
                " is trying to set amount > 1 on non-stackable multisell. Id: " + _listId +
                " entry: " + _entryId);

            player.setMultiSell(null);
            return ValueTask.CompletedTask;
        }

        ItemInfo? itemEnchantment = list.getItemEnchantment(_entryId);

        // Validate the requested item with its full stats.
		//@formatter:off
		if (itemEnchantment != null && (_amount > 1
		                                || itemEnchantment.getEnchantLevel() != _enchantLevel
		                                || itemEnchantment.getAttackElementType() != _attackAttribute
		                                || itemEnchantment.getAttackElementPower() != _attributePower
		                                || itemEnchantment.getAttributeDefence(AttributeType.FIRE) != _fireDefence
		                                || itemEnchantment.getAttributeDefence(AttributeType.WATER) != _waterDefence
		                                || itemEnchantment.getAttributeDefence(AttributeType.WIND) != _windDefence
		                                || itemEnchantment.getAttributeDefence(AttributeType.EARTH) != _earthDefence
		                                || itemEnchantment.getAttributeDefence(AttributeType.HOLY) != _holyDefence
		                                || itemEnchantment.getAttributeDefence(AttributeType.DARK) != _darkDefence
		                                || (itemEnchantment.getAugmentation() == null && (_augmentOption1 != 0 || _augmentOption2 != 0))
		                                || (itemEnchantment.getAugmentation() is {} augmentation && (augmentation.getOption1Id() != _augmentOption1 || augmentation.getOption2Id() != _augmentOption2))
		                                || (_soulCrystalOptions != null && !itemEnchantment.soulCrystalOptionsMatch(_soulCrystalOptions))
		                                || (_soulCrystalOptions == null && itemEnchantment.getSoulCrystalOptions().Count != 0)
		                                || (_soulCrystalSpecialOptions != null && !itemEnchantment.soulCrystalSpecialOptionsMatch(_soulCrystalSpecialOptions))
		                                || (_soulCrystalSpecialOptions == null && itemEnchantment.getSoulCrystalSpecialOptions().Count != 0)
			))
		{
			PacketLogger.Instance.Warn("Character: " + player.getName() + " is trying to upgrade equippable item, but the stats doesn't match. Id: " + _listId + " entry: " + _entryId);
			player.setMultiSell(null);
			return ValueTask.CompletedTask;
		}

		Clan? clan = player.getClan();
		PlayerInventory inventory = player.getInventory();

		try
		{
			int slots = 0;
			long weight = 0;
			foreach (ItemChanceHolder product in entry.getProducts())
			{
				if (product.Id < 0)
				{
					// Check if clan exists for clan reputation products.
					if (clan == null && (int)SpecialItemType.CLAN_REPUTATION == product.Id)
					{
						player.sendPacket(SystemMessageId.YOU_ARE_NOT_A_CLAN_MEMBER_2);
					    return ValueTask.CompletedTask;
					}

					continue;
				}

				ItemTemplate? template = ItemData.getInstance().getTemplate(product.Id);
				if (template == null)
				{
					player.setMultiSell(null);
				    return ValueTask.CompletedTask;
				}

				long totalCount = checked(list.getProductCount(product) * _amount);
				if (totalCount < 1 || totalCount > int.MaxValue)
				{
					player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
				    return ValueTask.CompletedTask;
				}

				if (!template.isStackable() || player.getInventory().getItemByItemId(product.Id) == null)
				{
					slots++;
				}

				weight += totalCount * template.getWeight();
				if (!inventory.validateWeight(weight))
				{
					player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT);
					return ValueTask.CompletedTask;
				}

				if (slots > 0 && !inventory.validateCapacity(slots))
				{
					player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
					return ValueTask.CompletedTask;
				}

				// If this is a chance multisell, reset slots and weight because only one item should be selected.
				// We just need to check if conditions for every item is met.
				if (list.isChanceMultisell())
				{
					slots = 0;
					weight = 0;
				}
			}

			// Check for enchanted item if its present in the inventory.
			if (itemEnchantment != null && inventory.getItemByObjectId(itemEnchantment.getObjectId()) == null)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.REQUIRED_S1);
				sm.Params.addItemName(itemEnchantment.getItem().Id);
				player.sendPacket(sm);
				return ValueTask.CompletedTask;
			}

			// Check for enchanted level and ingredient count requirements.
			List<ItemChanceHolder> summedIngredients = new();
			foreach (ItemChanceHolder ingredient in entry.getIngredients())
			{
				bool added = false;
				foreach (ItemChanceHolder summedIngredient in summedIngredients)
				{
					if (summedIngredient.Id == ingredient.Id &&
					    summedIngredient.getEnchantmentLevel() == ingredient.getEnchantmentLevel())
					{
						summedIngredients.Add(new ItemChanceHolder(ingredient.Id, ingredient.getChance(), ingredient.getCount() + summedIngredient.getCount(), ingredient.getEnchantmentLevel(), ingredient.isMaintainIngredient()));
						summedIngredients.Remove(summedIngredient);
						added = true;
					}
				}

				if (!added)
				{
					summedIngredients.Add(ingredient);
				}
			}

			foreach (ItemChanceHolder ingredient in summedIngredients)
			{
				if (ingredient.getEnchantmentLevel() > 0)
				{
					int found = 0;
					foreach (Item item in inventory.getAllItemsByItemId(ingredient.Id, ingredient.getEnchantmentLevel()))
					{
						if (item.getEnchantLevel() >= ingredient.getEnchantmentLevel())
						{
							found++;
						}
					}

					if (found < ingredient.getCount())
					{
                        ItemTemplate? ingredientTemplate = ItemData.getInstance().getTemplate(ingredient.Id);
                        string ingredientName = ingredientTemplate != null ? ingredientTemplate.getName() : "Unknown"; // TODO: refactor later

						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.REQUIRED_S1);
						sm.Params.addString("+" + ingredient.getEnchantmentLevel() + " " + ingredientName);
						player.sendPacket(sm);
					    return ValueTask.CompletedTask;
					}
				}
				else if (!checkIngredients(player, list, inventory, clan, ingredient.Id, checked(ingredient.getCount() * _amount)))
				{
				    return ValueTask.CompletedTask;
				}
			}

			bool itemEnchantmentProcessed = itemEnchantment == null;

			// Take all ingredients
			List<ItemInfo> itemsToUpdate = new List<ItemInfo>();
			foreach (ItemChanceHolder ingredient in entry.getIngredients())
			{
				if (ingredient.isMaintainIngredient())
				{
					continue;
				}

				long totalCount = checked(list.getIngredientCount(ingredient) * _amount);
				SpecialItemType specialItem = (SpecialItemType)ingredient.Id;
				if (Enum.IsDefined(specialItem))
				{
					// Take special item.
					switch (specialItem)
					{
						case SpecialItemType.CLAN_REPUTATION:
						{
							if (clan != null)
							{
								clan.takeReputationScore((int) totalCount);
								SystemMessagePacket smsg = new SystemMessagePacket(SystemMessageId.CLAN_REPUTATION_POINTS_S1_2);
								smsg.Params.addLong(totalCount);
								player.sendPacket(smsg);
							}
							break;
						}
						case SpecialItemType.FAME:
						{
							player.setFame(player.getFame() - (int) totalCount);
							player.updateUserInfo();
							// player.sendPacket(new ExBrExtraUserInfo(player));
							break;
						}
						case SpecialItemType.RAIDBOSS_POINTS:
						{
							player.setRaidbossPoints(player.getRaidbossPoints() - (int) totalCount);
							player.updateUserInfo();
							SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_CONSUMED_S1_RAID_POINTS);
							sm.Params.addLong(totalCount);
							player.sendPacket(sm);
							break;
						}
						case SpecialItemType.PC_CAFE_POINTS:
						{
							player.setPcCafePoints((int) (player.getPcCafePoints() - totalCount));
							player.sendPacket(new ExPcCafePointInfoPacket(player.getPcCafePoints(), (int) -totalCount, 1));
							break;
						}
						case SpecialItemType.HONOR_COINS:
						{
							player.setHonorCoins(player.getHonorCoins() - totalCount);
							break;
						}
						default:
						{
							PacketLogger.Instance.Warn("Character: " + player.getName() + " has suffered possible item loss by using multisell " + _listId + " which has non-implemented special ingredient with id: " + ingredient.Id + ".");
							return ValueTask.CompletedTask;
						}
					}
				}
				else if (ingredient.getEnchantmentLevel() > 0)
				{
					// Take the enchanted item.
					Item? destroyedItem = inventory.destroyItem("Multisell", inventory.getAllItemsByItemId(ingredient.Id, ingredient.getEnchantmentLevel()).First(), totalCount, player, npc);
					if (destroyedItem != null)
					{
						itemEnchantmentProcessed = true;
						itemsToUpdate.Add(new ItemInfo(destroyedItem));
						if (itemEnchantmentProcessed && destroyedItem.isEquipable()) // Will only consider first equipable ingredient.
						{
							itemEnchantment = new ItemInfo(destroyedItem);
						}
					}
					else
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.REQUIRED_S1);
						sm.Params.addItemName(ingredient.Id);
						player.sendPacket(sm);
						return ValueTask.CompletedTask;
					}
				}
				else if (!itemEnchantmentProcessed && itemEnchantment != null && itemEnchantment.getItem().Id == ingredient.Id)
				{
					// Take the enchanted item.
					Item? destroyedItem = inventory.destroyItem("Multisell", itemEnchantment.getObjectId(), totalCount, player, npc);
					if (destroyedItem != null)
					{
						itemEnchantmentProcessed = true;
						itemsToUpdate.Add(new ItemInfo(destroyedItem));
						if (itemEnchantmentProcessed && destroyedItem.isEquipable()) // Will only consider first equipable ingredient.
						{
							itemEnchantment = new ItemInfo(destroyedItem);
						}
					}
					else
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.REQUIRED_S1);
						sm.Params.addItemName(ingredient.Id);
						player.sendPacket(sm);
						return ValueTask.CompletedTask;
					}
				}
				else
				{
					// Take a regular item.
					Item? destroyedItem = inventory.destroyItemByItemId("Multisell", ingredient.Id, totalCount, player, npc);
					if (destroyedItem != null)
					{
						itemsToUpdate.Add(new ItemInfo(destroyedItem));
						if (itemEnchantmentProcessed && destroyedItem.isEquipable()) // Will only consider first equipable ingredient.
						{
							itemEnchantment = new ItemInfo(destroyedItem);
						}
					}
					else
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_NEED_S2_S1_S);
						sm.Params.addItemName(ingredient.Id);
						sm.Params.addLong(totalCount);
						player.sendPacket(sm);
						return ValueTask.CompletedTask;
					}
				}
			}

			// Generate the appropriate items
			List<ItemChanceHolder> products = entry.getProducts();
			if (list.isChanceMultisell())
			{
				ItemChanceHolder? randomProduct = ItemChanceHolder.getRandomHolder(entry.getProducts());
				products = randomProduct != null ? [randomProduct] : [];
			}

			foreach (ItemChanceHolder product in products)
			{
				long totalCount = checked(list.getProductCount(product) * _amount);
				SpecialItemType specialItem = (SpecialItemType)product.Id;
				if (Enum.IsDefined(specialItem))
				{
					// Give special item.
					switch (specialItem)
					{
						case SpecialItemType.CLAN_REPUTATION:
						{
							if (clan != null)
							{
								clan.addReputationScore((int) totalCount);
							}
							break;
						}
						case SpecialItemType.FAME:
						{
							player.setFame((int) (player.getFame() + totalCount));
							player.updateUserInfo();
							// player.sendPacket(new ExBrExtraUserInfo(player));
							break;
						}
						case SpecialItemType.RAIDBOSS_POINTS:
						{
							player.increaseRaidbossPoints((int) totalCount);
							player.updateUserInfo();
							break;
						}
						case SpecialItemType.HONOR_COINS:
						{
							player.setHonorCoins(player.getHonorCoins() + totalCount);
							break;
						}
						default:
						{
							PacketLogger.Instance.Warn("Character: " + player.getName() + " has suffered possible item loss by using multisell " + _listId + " which has non-implemented special product with id: " + product.Id + ".");
							return ValueTask.CompletedTask;
						}
					}
				}
				else
				{
					// Give item.
					Item? addedItem = inventory.addItem("Multisell", product.Id, totalCount, player, npc, false);
                    if (addedItem == null)
                    {
                        player.sendPacket(SystemMessageId.YOUR_INVENTORY_IS_FULL);
                        return ValueTask.CompletedTask;
                    }

					// Check if the newly given item should be enchanted.
					if (itemEnchantmentProcessed && list.isMaintainEnchantment() && itemEnchantment != null &&
					    addedItem.isEquipable() && addedItem.getTemplate().GetType() == itemEnchantment.getItem().GetType())
					{
						addedItem.setEnchantLevel(itemEnchantment.getEnchantLevel());
						addedItem.setAugmentation(itemEnchantment.getAugmentation(), false);
						if (addedItem.isWeapon())
						{
							if (itemEnchantment.getAttackElementPower() > 0)
							{
								addedItem.setAttribute(new AttributeHolder(itemEnchantment.getAttackElementType(), itemEnchantment.getAttackElementPower()), false);
							}
						}
						else
						{
							if (itemEnchantment.getAttributeDefence(AttributeType.FIRE) > 0)
							{
								addedItem.setAttribute(new AttributeHolder(AttributeType.FIRE, itemEnchantment.getAttributeDefence(AttributeType.FIRE)), false);
							}
							if (itemEnchantment.getAttributeDefence(AttributeType.WATER) > 0)
							{
								addedItem.setAttribute(new AttributeHolder(AttributeType.WATER, itemEnchantment.getAttributeDefence(AttributeType.WATER)), false);
							}
							if (itemEnchantment.getAttributeDefence(AttributeType.WIND) > 0)
							{
								addedItem.setAttribute(new AttributeHolder(AttributeType.WIND, itemEnchantment.getAttributeDefence(AttributeType.WIND)), false);
							}
							if (itemEnchantment.getAttributeDefence(AttributeType.EARTH) > 0)
							{
								addedItem.setAttribute(new AttributeHolder(AttributeType.EARTH, itemEnchantment.getAttributeDefence(AttributeType.EARTH)), false);
							}
							if (itemEnchantment.getAttributeDefence(AttributeType.HOLY) > 0)
							{
								addedItem.setAttribute(new AttributeHolder(AttributeType.HOLY, itemEnchantment.getAttributeDefence(AttributeType.HOLY)), false);
							}
							if (itemEnchantment.getAttributeDefence(AttributeType.DARK) > 0)
							{
								addedItem.setAttribute(new AttributeHolder(AttributeType.DARK, itemEnchantment.getAttributeDefence(AttributeType.DARK)), false);
							}
						}
						if (_soulCrystalOptions != null)
						{
							int pos = -1;
							foreach (EnsoulOption ensoul in _soulCrystalOptions)
							{
								pos++;
								addedItem.addSpecialAbility(ensoul, pos, 1, false);
							}
						}
						if (_soulCrystalSpecialOptions != null)
						{
							foreach (EnsoulOption ensoul in _soulCrystalSpecialOptions)
							{
								addedItem.addSpecialAbility(ensoul, 0, 2, false);
							}
						}

						addedItem.updateDatabase(true);

						// Mark that we have already upgraded the item.
						itemEnchantmentProcessed = false;
					}

					if (product.getEnchantmentLevel() > 0)
					{
						addedItem.setEnchantLevel(product.getEnchantmentLevel());
						addedItem.updateDatabase(true);
					}

					if (addedItem.getCount() > 1)
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
						sm.Params.addItemName(addedItem.Id);
						sm.Params.addLong(totalCount);
						player.sendPacket(sm);
					}
					else if (addedItem.getEnchantLevel() > 0)
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_VE_OBTAINED_S1_S2_2);
						sm.Params.addLong(addedItem.getEnchantLevel());
						sm.Params.addItemName(addedItem.Id);
						player.sendPacket(sm);
					}
					else
					{
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1);
						sm.Params.addItemName(addedItem);
						player.sendPacket(sm);
					}

					// Inventory update.
					itemsToUpdate.Add(new ItemInfo(addedItem));
					player.sendPacket(new ExMultiSellResultPacket(true, 0, (int) addedItem.getCount()));
				}
			}

			// Update inventory and weight.
			InventoryUpdatePacket iu = new InventoryUpdatePacket(itemsToUpdate);
			player.sendInventoryUpdate(iu);

			// Finally, give the tax to the castle.
			if (npc != null && list.isApplyTaxes())
			{
				long taxPaid = 0;
				foreach (ItemChanceHolder ingredient in entry.getIngredients())
				{
					if (ingredient.Id == Inventory.AdenaId)
					{
						taxPaid += checked((long)(ingredient.getCount() * list.getIngredientMultiplier() * list.getTaxRate() * _amount));
					}
				}
				if (taxPaid > 0)
				{
					npc.handleTaxPayment(taxPaid);
				}
			}
		}
		catch (ArithmeticException ae)
		{
            PacketLogger.Instance.Error(ae);
			player.sendPacket(SystemMessageId.YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED);
			return ValueTask.CompletedTask;
		}

		// Re-send multisell after successful exchange of inventory-only shown items.
		if (list.isInventoryOnly())
		{
			MultisellData.getInstance().separateAndSend(list.Id, player, npc, list.isInventoryOnly(), list.getProductMultiplier(), list.getIngredientMultiplier(), 0);
		}

		return ValueTask.CompletedTask;
	}

	/**
	 * @param player
	 * @param list
	 * @param inventory
	 * @param clan
	 * @param ingredientId
	 * @param totalCount
	 * @return {@code false} if ingredient amount is not enough, {@code true} otherwise.
	 */
	private bool checkIngredients(Player player, PreparedMultisellListHolder list, PlayerInventory inventory, Clan? clan, int ingredientId, long totalCount)
	{
		SpecialItemType specialItem = (SpecialItemType)ingredientId;
		if (Enum.IsDefined(specialItem))
		{
			// Check special item.
			switch (specialItem)
			{
				case SpecialItemType.CLAN_REPUTATION:
				{
					if (clan == null)
					{
						player.sendPacket(SystemMessageId.YOU_ARE_NOT_A_CLAN_MEMBER_2);
						return false;
					}

					if (!player.isClanLeader())
					{
						player.sendPacket(SystemMessageId.AVAILABLE_ONLY_TO_THE_CLAN_LEADER);
						return false;
					}

					if (clan.getReputationScore() < totalCount)
					{
						player.sendPacket(SystemMessageId.THE_CLAN_REPUTATION_IS_TOO_LOW);
						return false;
					}

					return true;
				}
				case SpecialItemType.FAME:
				{
					if (player.getFame() < totalCount)
					{
						player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_FAME_TO_DO_THAT);
						return false;
					}
					return true;
				}
				case SpecialItemType.RAIDBOSS_POINTS:
				{
					if (player.getRaidbossPoints() < totalCount)
					{
						player.sendPacket(SystemMessageId.NOT_ENOUGH_RAID_POINTS);
						return false;
					}
					return true;
				}
				case SpecialItemType.PC_CAFE_POINTS:
				{
					if (player.getPcCafePoints() < totalCount)
					{
						player.sendPacket(SystemMessageId.YOU_ARE_SHORT_OF_PA_POINTS);
						return false;
					}
					return true;
				}
				case SpecialItemType.HONOR_COINS:
				{
					if (player.getHonorCoins() < totalCount)
					{
						player.sendMessage("You are short of Honor Points.");
						return false;
					}
					return true;
				}
				default:
				{
					PacketLogger.Instance.Warn("Multisell: " + _listId + " is using a non-implemented special ingredient with id: " + ingredientId + ".");
					return false;
				}
			}
		}

		// Check if the necessary items are there. If list maintains enchantment, allow all enchanted items, otherwise only unenchanted. TODO: Check how retail does it.
		if (inventory.getInventoryItemCount(ingredientId, list.isMaintainEnchantment() ? -1 : 0, false) < totalCount)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_NEED_S2_S1_S);
			sm.Params.addItemName(ingredientId);
			sm.Params.addLong(totalCount);
			player.sendPacket(sm);
			return false;
		}

		return true;
    }
}