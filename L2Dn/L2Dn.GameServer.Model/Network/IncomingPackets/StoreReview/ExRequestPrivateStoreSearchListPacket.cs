using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.StoreReviews;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.StoreReview;

public struct ExRequestPrivateStoreSearchListPacket: IIncomingPacket<GameSession>
{
    public const int MAX_ITEM_PER_PAGE = 120;

    private string _searchWord;
    private StoreType _storeType;
    private StoreItemType _itemType;
    private StoreSubItemType _itemSubtype;
    private int _searchCollection;

    public void ReadContent(PacketBitReader reader)
    {
        _searchWord = reader.ReadSizedString();
        _storeType = (StoreType)reader.ReadByte();
        _itemType = (StoreItemType)reader.ReadByte();
        _itemSubtype = (StoreSubItemType)reader.ReadByte();
        _searchCollection = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		ICollection<Player> stores = World.getInstance().getSellingOrBuyingPlayers();
		List<ShopItem> items = new();
		List<int> itemIds = new();

		StoreType storeType = _storeType;
		string searchWord = _searchWord;
		StoreItemType itemType = _itemType;
		StoreSubItemType itemSubtype = _itemSubtype;
		int searchCollection = _searchCollection;
		stores.ForEach(vendor =>
		{
			foreach (TradeItem item in vendor.getPrivateStoreType() == PrivateStoreType.BUY
				         ? vendor.getBuyList().getItems()
				         : vendor.getSellList().getItems())
			{
				// Filter by storeType.
				if (storeType == StoreType.ALL ||
				    (storeType == StoreType.SELL && (vendor.getPrivateStoreType() == PrivateStoreType.SELL ||
				                                     vendor.getPrivateStoreType() == PrivateStoreType.PACKAGE_SELL)) ||
				    (storeType == StoreType.BUY && vendor.getPrivateStoreType() == PrivateStoreType.BUY))
				{
					if (isItemVisibleForShop(item, itemType, itemSubtype, searchCollection))
					{
						// Filter by Word if supplied.
						if (searchWord.equals("") || (!searchWord.equals("") &&
						                               item.getItem().getName().toLowerCase()
							                               .contains(searchWord.toLowerCase())))
						{
							items.Add(new ShopItem(item, vendor, vendor.getPrivateStoreType()));
							itemIds.Add(item.getItem().Id);
						}
					}
				}
			}
		});

		int nSize = items.Count;
		int maxPage = Math.Max(1,
			nSize / (MAX_ITEM_PER_PAGE * 1F) > nSize / MAX_ITEM_PER_PAGE
				? nSize / MAX_ITEM_PER_PAGE + 1
				: nSize / MAX_ITEM_PER_PAGE);

		for (int pageIndex = 1; pageIndex <= maxPage; pageIndex++)
		{
			int nsize = pageIndex == maxPage
				? nSize % MAX_ITEM_PER_PAGE > 0 || nSize == 0 ? nSize % MAX_ITEM_PER_PAGE : MAX_ITEM_PER_PAGE
				: MAX_ITEM_PER_PAGE;

			player.sendPacket(new ExPrivateStoreSearchItemPacket(pageIndex, maxPage, nsize, items));
		}

		List<PrivateStoreHistoryManager.ItemHistoryTransaction> history = new();
		List<PrivateStoreHistoryManager.ItemHistoryTransaction> historyTemp = new();
		PrivateStoreHistoryManager.getInstance().getHistory().ForEach(transaction =>
		{
			if (itemIds.Contains(transaction.getItemId()))
			{
				history.Add(transaction);
			}
		});

		int page = 1;
		maxPage = Math.Max(1, history.Count / (MAX_ITEM_PER_PAGE * 1F) > history.Count / MAX_ITEM_PER_PAGE ? history.Count / MAX_ITEM_PER_PAGE + 1 : history.Count / MAX_ITEM_PER_PAGE);

		for (int index = 0; index < history.Count; index++)
		{
			historyTemp.Add(history[index]);

			if (index == history.Count - 1 || index == MAX_ITEM_PER_PAGE - 1 || (index > 0 && index % (MAX_ITEM_PER_PAGE - 1) == 0))
			{
				player.sendPacket(new ExPrivateStoreSearchHistoryPacket(page, maxPage, historyTemp));
				page++;
				historyTemp.Clear();
			}
		}

		if (page == 1)
		{
			player.sendPacket(new ExPrivateStoreSearchHistoryPacket(1, 1, historyTemp));
		}

		return ValueTask.CompletedTask;
	}

	private static bool isItemVisibleForShop(TradeItem item, StoreItemType itemType, StoreSubItemType itemSubtype, int searchCollection)
	{
		/**
		 * Equipement
		 */
		if (itemType == StoreItemType.EQUIPMENT && itemSubtype == StoreSubItemType.ALL && searchCollection == 0)
		{
			// Equipment - All
			return item.getItem().isEquipable();
		}
		if (itemType == StoreItemType.EQUIPMENT && itemSubtype == StoreSubItemType.WEAPON && searchCollection == 0)
		{
			// Equipment - Weapon
			return item.getItem().isEquipable() && item.getItem().isWeapon();
		}
		if (itemType == StoreItemType.EQUIPMENT && itemSubtype == StoreSubItemType.ARMOR && searchCollection == 0)
		{
			// Equipment - Armor
			return item.getItem().isEquipable() && isEquipmentArmor(item.getItem());
		}
		if (itemType == StoreItemType.EQUIPMENT && itemSubtype == StoreSubItemType.ACCESSORY && searchCollection == 0)
		{
			// Equipment - Accessory
			return item.getItem().isEquipable() && isAccessory(item.getItem());
		}
		if (itemType == StoreItemType.EQUIPMENT && itemSubtype == StoreSubItemType.EQUIPMENT_MISC && searchCollection == 0)
		{
			// Equipment - Misc
			return item.getItem().isEquipable() && !item.getItem().isWeapon() && !isEquipmentArmor(item.getItem()) && !isAccessory(item.getItem());
		}

		/**
		 * Exping / Enhancement
		 */
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && itemSubtype == StoreSubItemType.ALL && searchCollection == 0)
		{
			// Exping / Enhancement - All
			return isEnhancementItem(item.getItem());
		}
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && itemSubtype == StoreSubItemType.ENCHANT_SCROLL && searchCollection == 0)
		{
			// Exping / Enhancement - Enchant Scroll
			return isEnchantScroll(item.getItem());
		}
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && itemSubtype == StoreSubItemType.CRYSTAL && searchCollection == 0)
		{
			// Exping / Enhancement - Crystal
			return isCrystal(item.getItem());
		}
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && itemSubtype == StoreSubItemType.LIFE_STONE && searchCollection == 0)
		{
			// Exping / Enhancement - Life Stone
			return isLifeStone(item.getItem());
		}
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && itemSubtype == StoreSubItemType.DYES && searchCollection == 0)
		{
			// Exping / Enhancement - Dyes
			return isDye(item.getItem());
		}
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && itemSubtype == StoreSubItemType.SPELLBOOK && searchCollection == 0)
		{
			// Exping / Enhancement - SpellBooks
			return isSpellBook(item.getItem());
		}
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && itemSubtype == StoreSubItemType.ENHANCEMENT_MISC && searchCollection == 0)
		{
			// Exping / Enhancement - Misc
			return isEnhancementMisc(item.getItem());
		}

		/**
		 * Groceries
		 */
		if (itemType == StoreItemType.GROCERY_OR_COLLECTION_MISC && itemSubtype == StoreSubItemType.ALL && searchCollection == 0)
		{
			// Groceries - All
			return item.getItem().isPotion() || item.getItem().isScroll() || isTicket(item.getItem()) || isPackOrCraft(item.getItem()) || isGroceryMisc(item.getItem());
		}
		if (itemType == StoreItemType.GROCERY_OR_COLLECTION_MISC && itemSubtype == StoreSubItemType.POTION_SCROLL && searchCollection == 0)
		{
			// Groceries - Potion/Scroll
			return item.getItem().isPotion() || item.getItem().isScroll();
		}
		if (itemType == StoreItemType.GROCERY_OR_COLLECTION_MISC && itemSubtype == StoreSubItemType.TICKET && searchCollection == 0)
		{
			// Groceries - Ticket
			return isTicket(item.getItem());
		}
		if (itemType == StoreItemType.GROCERY_OR_COLLECTION_MISC && itemSubtype == StoreSubItemType.PACK_CRAFT && searchCollection == 0)
		{
			// Groceries - Pack/Craft
			return isPackOrCraft(item.getItem());
		}
		if (itemType == StoreItemType.GROCERY_OR_COLLECTION_MISC && itemSubtype == StoreSubItemType.GROCERY_MISC && searchCollection == 0)
		{
			// Groceries - Misc
			return isGroceryMisc(item.getItem());
		}

		/**
		 * Collections
		 */
		if (itemType == StoreItemType.ALL && searchCollection == 1)
		{
			// Collections - All
			return isCollection(item.getItem());
		}
		if (itemType == StoreItemType.EQUIPMENT && searchCollection == 1)
		{
			// Collections - Equipement
			return isCollectionEquipement(item.getItem());
		}
		if (itemType == StoreItemType.ENHANCEMENT_OR_EXPING && searchCollection == 1)
		{
			// Collections - Enchanted Item
			return isCollectionEnchanted(item.getItem());
		}
		if (itemType == StoreItemType.GROCERY_OR_COLLECTION_MISC && searchCollection == 1)
		{
			// Collections - Misc
			return isCollectionMisc(item.getItem());
		}

		return true;
	}

	private static bool isEquipmentArmor(ItemTemplate item)
	{
		return item.isArmor() && (item.getBodyPart() == ItemTemplate.SLOT_CHEST || item.getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR || item.getBodyPart() == ItemTemplate.SLOT_HEAD || item.getBodyPart() == ItemTemplate.SLOT_LEGS || item.getBodyPart() == ItemTemplate.SLOT_FEET || item.getBodyPart() == ItemTemplate.SLOT_GLOVES || item.getBodyPart() == (ItemTemplate.SLOT_CHEST | ItemTemplate.SLOT_LEGS));
	}

	private static bool isAccessory(ItemTemplate item)
	{
		return item.isArmor() && (item.getBodyPart() == (ItemTemplate.SLOT_L_BRACELET | ItemTemplate.SLOT_R_BRACELET | ItemTemplate.SLOT_BROOCH) || item.getBodyPart() == (ItemTemplate.SLOT_R_FINGER | ItemTemplate.SLOT_L_FINGER) || item.getBodyPart() == ItemTemplate.SLOT_NECK || item.getBodyPart() == (ItemTemplate.SLOT_R_EAR | ItemTemplate.SLOT_L_EAR));
	}

	private static bool isEnchantScroll(ItemTemplate item)
	{
		if (!(item is EtcItem))
		{
			return false;
		}

		IItemHandler? ih = ItemHandler.getInstance().getHandler((EtcItem) item);

		return ih != null && ih.GetType().Name.equals("EnchantScrolls");
	}

	private static bool isCrystal(ItemTemplate item)
	{
		return EnsoulData.getInstance().getStone(item.Id) != null;
	}

	private static bool isLifeStone(ItemTemplate item)
	{
		return VariationData.getInstance().hasVariation(item.Id);
	}

	private static bool isDye(ItemTemplate item)
	{
		return HennaData.getInstance().getHennaByItemId(item.Id) != null;
	}

	private static bool isSpellBook(ItemTemplate item)
	{
		return item.getName().contains("Spellbook: ");
	}

	private static bool isEnhancementMisc(ItemTemplate item)
	{
		return item.Id >= 91031 && item.Id <= 91038;
	}

	private static bool isEnhancementItem(ItemTemplate item)
	{
		return isEnchantScroll(item) || isCrystal(item) || isLifeStone(item) || isDye(item) || isSpellBook(item) ||
		       isEnhancementMisc(item);
	}

	private static bool isTicket(ItemTemplate item)
	{
		return item.Id == 90045 || item.Id == 91462 || item.Id == 91463 ||
		       item.Id == 91972 || item.Id == 93903;
	}

	private static bool isPackOrCraft(ItemTemplate item)
	{
        if (item.Id == 92477 || item.Id == 91462 || item.Id == 92478 || item.Id == 92479 ||
            item.Id == 92480 || item.Id == 92481 || item.Id == 49756 || item.Id == 93906 ||
            item.Id == 93907 || item.Id == 93908 || item.Id == 93909 || item.Id == 93910 ||
            item.Id == 91076)
        {
            return true;
        }

        if (!(item is EtcItem))
		{
			return false;
		}

		IItemHandler? ih = ItemHandler.getInstance().getHandler((EtcItem) item);
		return ih != null && ih.GetType().Name.equals("ExtractableItems");
	}

	private static bool isGroceryMisc(ItemTemplate item)
	{
		// Kinda fallback trash category to ensure no skipping any items.
		return !item.isEquipable() && !isEnhancementItem(item) && !isCollection(item) && !item.isPotion() &&
		       !item.isScroll() && !isTicket(item) && !isPackOrCraft(item);
	}

	private static bool isCollection(ItemTemplate item)
	{
		foreach (CollectionDataHolder collectionHolder in CollectionData.getInstance().getCollections())
		{
			foreach (ItemEnchantHolder itemData in collectionHolder.getItems())
			{
				if (itemData.Id == item.Id)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool isCollectionEquipement(ItemTemplate item)
	{
		return isCollection(item) && item.isEquipable();
	}

	private static bool isCollectionEnchanted(ItemTemplate item)
	{
		return isCollection(item) && item.getName().contains("Spellbook: ");
	}

	private static bool isCollectionMisc(ItemTemplate item)
	{
		return item.Id >= 93906 && item.Id <= 93910;
	}

	private enum StoreType
	{
		SELL = 0x00,
		BUY = 0x01,
		ALL = 0x03
	}

	private enum StoreItemType
	{
		ALL = 0xFF,
		EQUIPMENT = 0x00,
		ENHANCEMENT_OR_EXPING = 0x02,
		GROCERY_OR_COLLECTION_MISC = 0x04
	}

	private enum StoreSubItemType
	{
		ALL = 0xFF,
		WEAPON = 0x00,
		ARMOR = 0x01,
		ACCESSORY = 0x02,
		EQUIPMENT_MISC = 0x03,
		ENCHANT_SCROLL = 0x08,
		LIFE_STONE = 0x0F,
		DYES = 0x10,
		CRYSTAL = 0x11,
		SPELLBOOK = 0x12,
		ENHANCEMENT_MISC = 0x13,
		POTION_SCROLL = 0x14,
		TICKET = 0x15,
		PACK_CRAFT = 0x16,
		GROCERY_MISC = 0x18
	}
}