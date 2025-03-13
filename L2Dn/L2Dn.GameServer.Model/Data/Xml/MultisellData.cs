using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Globalization;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public class MultisellData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(MultisellData));

	public const int PAGE_SIZE = 40;

	private FrozenDictionary<int, MultisellListHolder> _multiSellLists =
		FrozenDictionary<int, MultisellListHolder>.Empty;

	private MultisellData()
	{
		load();
	}

	public void load()
	{
		IEnumerable<(string FilePath, XmlMultiSellList Document)> files =
			LoadXmlDocuments<XmlMultiSellList>(DataFileLocation.Data, "multisell")
				.Concat(LoadXmlDocuments<XmlMultiSellList>(DataFileLocation.Data, "multisell/items"));

		if (Config.CUSTOM_MULTISELL_LOAD)
			files = files.Concat(LoadXmlDocuments<XmlMultiSellList>(DataFileLocation.Data, "multisell/custom"));

		_multiSellLists = files.Select(x => loadFile(x.FilePath, x.Document)).Where(l => l != null)
			.ToFrozenDictionary(x => x!.getId())!;

		_logger.Info(GetType().Name + ": Loaded " + _multiSellLists.Count + " multisell lists.");
	}

	private MultisellListHolder? loadFile(string filePath, XmlMultiSellList multiSellList)
	{
		string fileName = Path.GetFileNameWithoutExtension(filePath);
		if (!int.TryParse(fileName, CultureInfo.InvariantCulture, out int listId))
			return null;

		EnchantItemGroup? magicWeaponGroup = EnchantItemGroupsData.getInstance().getItemGroup("MAGE_WEAPON_GROUP");
		int magicWeaponGroupMax = magicWeaponGroup != null ? magicWeaponGroup.getMaximumEnchant() : -2;
		EnchantItemGroup? weapongroup = EnchantItemGroupsData.getInstance().getItemGroup("FIGHTER_WEAPON_GROUP");
		int weaponGroupMax = weapongroup != null ? weapongroup.getMaximumEnchant() : -2;
		EnchantItemGroup? fullArmorGroup = EnchantItemGroupsData.getInstance().getItemGroup("FULL_ARMOR_GROUP");
		int fullArmorGroupMax = fullArmorGroup != null ? fullArmorGroup.getMaximumEnchant() : -2;
		EnchantItemGroup? armorGroup = EnchantItemGroupsData.getInstance().getItemGroup("ARMOR_GROUP");
		int armorGroupMax = armorGroup != null ? armorGroup.getMaximumEnchant() : -2;

		try
		{
			List<int> npcs = multiSellList.Npcs;
			List<MultisellEntryHolder> entries = new();
			int entryCounter = 0;

			foreach (XmlMultiSellListItem itemEntry in multiSellList.Items)
			{
				long totalPrice = 0;
				int lastIngredientId = 0;
				long lastIngredientCount = 0;
				entryCounter++;

				List<ItemChanceHolder> ingredients = new();
				List<ItemChanceHolder> products = new();

				foreach (XmlMultiSellListIngredient ingredientEntry in itemEntry.Ingredients)
				{
					ItemChanceHolder ingredient = new(ingredientEntry.ItemId, 0, ingredientEntry.Count,
						ingredientEntry.EnchantLevel, ingredientEntry.MaintainIngredient);

					if (!ItemExists(ingredient))
					{
						_logger.Warn("Invalid ingredient id or count for itemId: " + ingredient.getId() +
						             ", count: " + ingredient.getCount() + " in list: " + listId);

						continue;
					}

					ingredients.Add(ingredient);
					lastIngredientId = ingredientEntry.ItemId;
					lastIngredientCount = ingredientEntry.Count;
				}

				double totalChance = 0;
				foreach (XmlMultiSellListProduct productEntry in itemEntry.Products)
				{
					byte enchantmentLevel = productEntry.EnchantLevel;
					ItemTemplate? item = ItemData.getInstance().getTemplate(productEntry.ItemId);
					if (productEntry.EnchantLevel > 0 && item != null)
					{
						if (item.isWeapon())
						{
							enchantmentLevel = (byte)Math.Min(enchantmentLevel,
								item.isMagicWeapon()
									?
									magicWeaponGroupMax > -2 ? magicWeaponGroupMax : enchantmentLevel
									: weaponGroupMax > -2
										? weaponGroupMax
										: enchantmentLevel);
						}
						else if (item.isArmor())
						{
							enchantmentLevel = (byte)Math.Min(enchantmentLevel,
								item.getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR
									?
									fullArmorGroupMax > -2 ? fullArmorGroupMax : enchantmentLevel
									: armorGroupMax > -2
										? armorGroupMax
										: enchantmentLevel);
						}
					}

					// Check chance only of items that have set chance.
					// Items without chance are used for displaying purposes.
					if (productEntry is { ChanceSpecified: true, Chance: < 0 or > 100 })
					{
						_logger.Warn("Invalid chance for itemId: " + productEntry.ItemId + ", count: " +
						             productEntry.Count + ", chance: " + productEntry.Chance + " in list: " + listId);

						return null;
					}

					if (productEntry.ChanceSpecified)
						totalChance += productEntry.Chance;

					ItemChanceHolder product = new(productEntry.ItemId, productEntry.Chance, productEntry.Count,
						enchantmentLevel);

					if (!ItemExists(product))
					{
						_logger.Warn("Invalid product id or count for itemId: " + product.getId() + ", count: " +
						             product.getCount() + " in list: " + listId);

						continue;
					}

					products.Add(product);

					if (item != null)
					{
						double? chance = product.getChance();
						if (chance != null)
							totalPrice += (long)(item.getReferencePrice() / 2.0 * product.getCount() *
							                     (chance.Value / 100.0));
						else
							totalPrice += item.getReferencePrice() / 2 * product.getCount();
					}
				}

				if (totalChance > 100)
				{
					_logger.Warn("Products' total chance of " + totalChance + "% exceeds 100% for list: " + listId +
					             " at entry " + entries.Count + 1 + ".");
				}

				// Check if buy price is lower than sell price.
				// Only applies when there is only one ingredient and it is adena.
				if (Config.CORRECT_PRICES && ingredients.Count == 1 && lastIngredientId == 57 &&
				    lastIngredientCount < totalPrice)
				{
					_logger.Warn("Buy price " + lastIngredientCount + " is less than sell price " + totalPrice +
					             " at entry " + entryCounter + " of multisell " + listId + ".");
					// Adjust price.
					ItemChanceHolder ingredient = new(57, 0, totalPrice, 0,
						ingredients[0].isMaintainIngredient());
					ingredients.Clear();
					ingredients.Add(ingredient);
				}

				MultisellEntryHolder entry = new MultisellEntryHolder(ingredients, products);
				entries.Add(entry);
			}

			return new MultisellListHolder(listId, multiSellList.IsChanceMultiSell, multiSellList.ApplyTaxes,
				multiSellList.MaintainEnchantment, multiSellList.IngredientMultiplier, multiSellList.ProductMultiplier,
				entries.ToImmutableArray(), npcs.ToFrozenSet());
		}
		catch (Exception e)
		{
			_logger.Error(GetType().Name + ": Error in file " + filePath + ", " + e);
			return null;
		}
	}

	/**
	 * This will generate the multisell list for the items.<br>
	 * There exist various parameters in multisells that affect the way they will appear:
	 * <ol>
	 * <li>Inventory only:
	 * <ul>
	 * <li>If true, only show items of the multisell for which the "primary" ingredients are already in the player's inventory. By "primary" ingredients we mean weapon and armor.</li>
	 * <li>If false, show the entire list.</li>
	 * </ul>
	 * </li>
	 * <li>Maintain enchantment: presumably, only lists with "inventory only" set to true should sometimes have this as true. This makes no sense otherwise...
	 * <ul>
	 * <li>If true, then the product will match the enchantment level of the ingredient.<br>
	 * If the player has multiple items that match the ingredient list but the enchantment levels differ, then the entries need to be duplicated to show the products and ingredients for each enchantment level.<br>
	 * For example: If the player has a crystal staff +1 and a crystal staff +3 and goes to exchange it at the mammon, the list should have all exchange possibilities for the +1 staff, followed by all possibilities for the +3 staff.</li>
	 * <li>If false, then any level ingredient will be considered equal and product will always be at +0</li>
	 * </ul>
	 * </li>
	 * <li>Apply taxes: Uses the "taxIngredient" entry in order to add a certain amount of adena to the ingredients.
	 * <li>
	 * <li>Additional product and ingredient multipliers.</li>
	 * </ol>
	 * @param listId
	 * @param player
	 * @param npc
	 * @param inventoryOnly
	 * @param ingredientMultiplierValue
	 * @param productMultiplierValue
	 * @param type
	 */
	public void separateAndSend(int listId, Player player, Npc? npc, bool inventoryOnly,
		double? ingredientMultiplierValue, double? productMultiplierValue, int type)
	{
		MultisellListHolder? template = _multiSellLists.GetValueOrDefault(listId);
		if (template == null)
		{
			_logger.Warn("Can't find list id: " + listId + " requested by player: " + player.getName() + ", npcId: " +
			            (npc != null ? npc.getId() : 0));
			return;
		}

        if (!template.isNpcAllowed(-1) && (npc == null || !template.isNpcAllowed(npc.getId())))
        {
            if (player.isGM())
            {
                player.sendMessage("Multisell " + listId +
                    " is restricted. Under current conditions cannot be used. Only GMs are allowed to use it.");
            }
            else
            {
                _logger.Warn(GetType().Name + ": " + player + " attempted to open multisell " + listId + " from npc " +
                    npc + " which is not allowed!");

                return;
            }
        }

        // Check if ingredient/product multipliers are set, if not, set them to the template value.
		double ingredientMultiplier = ingredientMultiplierValue ?? template.getIngredientMultiplier();
		double productMultiplier = productMultiplierValue ?? template.getProductMultiplier();
		PreparedMultisellListHolder list = new(template, inventoryOnly, player.getInventory(), npc,
			ingredientMultiplier, productMultiplier);

		int index = 0;
		do
		{
			// send list at least once even if size = 0
			player.sendPacket(new MultiSellListPacket(player, list, index, type));
			index += PAGE_SIZE;
		} while (index < list.getEntries().Length);

		player.setMultiSell(list);
	}

	public void separateAndSend(int listId, Player player, Npc? npc, bool inventoryOnly)
	{
		separateAndSend(listId, player, npc, inventoryOnly, null, null, 0);
	}

	private static bool ItemExists(ItemHolder holder)
	{
		SpecialItemType specialItem = (SpecialItemType)holder.getId();
		if (Enum.IsDefined(specialItem))
		{
			return true;
		}

		ItemTemplate? template = ItemData.getInstance().getTemplate(holder.getId());
		return template != null && (template.isStackable() ? holder.getCount() >= 1 : holder.getCount() == 1);
	}

	public MultisellListHolder? getMultisell(int id)
	{
		return _multiSellLists.GetValueOrDefault(id);
	}

	public static MultisellData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly MultisellData INSTANCE = new();
	}
}