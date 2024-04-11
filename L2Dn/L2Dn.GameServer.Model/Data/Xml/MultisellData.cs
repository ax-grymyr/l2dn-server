using System.Globalization;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public class MultisellData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MultisellData));
	
	public const int PAGE_SIZE = 40;
	
	private readonly Map<int, MultisellListHolder> _multisells = new();
	
	protected MultisellData()
	{
		load();
	}
	
	public void load()
	{
		_multisells.clear();
		
		LoadXmlDocuments(DataFileLocation.Data, "multisell").ForEach(t =>
		{
			t.Document.Elements("list").ForEach(x => loadFile(t.FilePath, x));
		});

		LoadXmlDocuments(DataFileLocation.Data, "multisell/items").ForEach(t =>
		{
			t.Document.Elements("list").ForEach(x => loadFile(t.FilePath, x));
		});

		if (Config.CUSTOM_MULTISELL_LOAD)
		{
			LoadXmlDocuments(DataFileLocation.Data, "multisell/custom").ForEach(t =>
			{
				t.Document.Elements("list").ForEach(x => loadFile(t.FilePath, x));
			});
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _multisells.size() + " multisell lists.");
	}

	private void loadFile(string filePath, XElement element)
	{
		string fileName = Path.GetFileNameWithoutExtension(filePath);
		if (!int.TryParse(fileName, CultureInfo.InvariantCulture, out int listId))
			return;

		EnchantItemGroup magicWeaponGroup = EnchantItemGroupsData.getInstance().getItemGroup("MAGE_WEAPON_GROUP");
		int magicWeaponGroupMax = magicWeaponGroup != null ? magicWeaponGroup.getMaximumEnchant() : -2;
		EnchantItemGroup weapongroup = EnchantItemGroupsData.getInstance().getItemGroup("FIGHTER_WEAPON_GROUP");
		int weaponGroupMax = weapongroup != null ? weapongroup.getMaximumEnchant() : -2;
		EnchantItemGroup fullArmorGroup = EnchantItemGroupsData.getInstance().getItemGroup("FULL_ARMOR_GROUP");
		int fullArmorGroupMax = fullArmorGroup != null ? fullArmorGroup.getMaximumEnchant() : -2;
		EnchantItemGroup armorGroup = EnchantItemGroupsData.getInstance().getItemGroup("ARMOR_GROUP");
		int armorGroupMax = armorGroup != null ? armorGroup.getMaximumEnchant() : -2;

		try
		{
			bool isChanceMultisell = element.Attribute("isChanceMultisell").GetBoolean(false);
			bool applyTaxes = element.Attribute("applyTaxes").GetBoolean(false);
			bool maintainEnchantment = element.Attribute("maintainEnchantment").GetBoolean(false);
			double ingredientMultiplier = element.Attribute("ingredientMultiplier").GetDouble(1.0);
			double productMultiplier = element.Attribute("productMultiplier").GetDouble(1.0);
			
			IEnumerable<int> npcIds = element.Elements("npcs").Elements("npc").Select(e => (int)e);
			Set<int> npcs = new();
			element.Elements("npcs").Elements("npc").Select(e => (int)e).ForEach(n => npcs.add(n));

			List<MultisellEntryHolder> entries = new();
			int entryCounter = 0;

			element.Elements("item").ForEach(el =>
			{
				long totalPrice = 0;
				int lastIngredientId = 0;
				long lastIngredientCount = 0;
				entryCounter++;

				List<ItemChanceHolder> ingredients = new();
				List<ItemChanceHolder> products = new();
				MultisellEntryHolder entry = new MultisellEntryHolder(ingredients, products);
				
				el.Elements("ingredient").ForEach(e =>
				{
					int id = e.GetAttributeValueAsInt32("id");
					long count = e.GetAttributeValueAsInt64("count");
					byte enchantmentLevel = e.Attribute("enchantmentLevel").GetByte(0);
					Boolean maintainIngredient = e.Attribute("maintainIngredient").GetBoolean(false);
					ItemChanceHolder ingredient = new ItemChanceHolder(id, 0, count, enchantmentLevel, maintainIngredient);
					if (itemExists(ingredient))
					{
						ingredients.add(ingredient);

						lastIngredientId = id;
						lastIngredientCount = count;
					}
					else
					{
						LOGGER.Warn("Invalid ingredient id or count for itemId: " + ingredient.getId() +
						            ", count: " + ingredient.getCount() + " in list: " + listId);
					}
				});

				el.Elements("production").ForEach(e =>
				{
					int id = e.GetAttributeValueAsInt32("id");
					long count = e.GetAttributeValueAsInt64("count");
					double chance = e.Attribute("chance").GetDouble(double.NaN);
					byte enchantmentLevel = e.Attribute("enchantmentLevel").GetByte(0);
					if (enchantmentLevel > 0)
					{
						ItemTemplate item = ItemData.getInstance().getTemplate(id);
						if (item != null)
						{
							if (item.isWeapon())
							{
								enchantmentLevel = (byte)Math.Min(enchantmentLevel,
									item.isMagicWeapon()
										? magicWeaponGroupMax > -2 ? magicWeaponGroupMax : enchantmentLevel
										: weaponGroupMax > -2
											? weaponGroupMax
											: enchantmentLevel);
							}
							else if (item.isArmor())
							{
								enchantmentLevel = (byte)Math.Min(enchantmentLevel,
									item.getBodyPart() == ItemTemplate.SLOT_FULL_ARMOR
										? fullArmorGroupMax > -2 ? fullArmorGroupMax : enchantmentLevel
										: armorGroupMax > -2
											? armorGroupMax
											: enchantmentLevel);
							}
						}
					}

					ItemChanceHolder product = new ItemChanceHolder(id, chance, count, enchantmentLevel);
					if (itemExists(product))
					{
						// Check chance only of items that have set chance. Items without chance (NaN) are used for displaying purposes.
						if ((!Double.IsNaN(chance) && (chance < 0)) || (chance > 100))
						{
							LOGGER.Warn("Invalid chance for itemId: " + product.getId() + ", count: " +
							            product.getCount() + ", chance: " + chance + " in list: " + listId);
							return;
						}

						products.add(product);

						ItemTemplate item = ItemData.getInstance().getTemplate(id);
						if (item != null)
						{
							if (chance > 0)
								totalPrice = (long)(totalPrice + ((item.getReferencePrice() / 2.0) * count) * (chance / 100));
							else
								totalPrice += ((item.getReferencePrice() / 2) * count);
						}
					}
					else
					{
						LOGGER.Warn("Invalid product id or count for itemId: " + product.getId() + ", count: " +
						            product.getCount() + " in list: " + listId);
					}
				});

				double totalChance = products.Where(i => !Double.IsNaN(i.getChance())).Select(i => i.getChance()).Sum();
				if (totalChance > 100)
				{
					LOGGER.Warn("Products' total chance of " + totalChance + "% exceeds 100% for list: " + listId +
					            " at entry " + entries.size() + 1 + ".");
				}

				// Check if buy price is lower than sell price.
				// Only applies when there is only one ingredient and it is adena.
				if (Config.CORRECT_PRICES && (ingredients.size() == 1) && (lastIngredientId == 57) &&
				    (lastIngredientCount < totalPrice))
				{
					LOGGER.Warn("Buy price " + lastIngredientCount + " is less than sell price " + totalPrice +
					            " at entry " + entryCounter + " of multisell " + listId + ".");
					// Adjust price.
					ItemChanceHolder ingredient = new ItemChanceHolder(57, 0, totalPrice, (byte)0,
						ingredients.get(0).isMaintainIngredient());
					ingredients.Clear();
					ingredients.add(ingredient);
				}

				entries.add(entry);
			});


			_multisells.put(listId,
				new MultisellListHolder(listId, isChanceMultisell, applyTaxes, maintainEnchantment,
					ingredientMultiplier, productMultiplier, entries, npcs));
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error in file " + filePath, e);
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
	public void separateAndSend(int listId, Player player, Npc npc, bool inventoryOnly,
		double ingredientMultiplierValue, double productMultiplierValue, int type)
	{
		MultisellListHolder template = _multisells.get(listId);
		if (template == null)
		{
			LOGGER.Warn("Can't find list id: " + listId + " requested by player: " + player.getName() + ", npcId: " +
			            (npc != null ? npc.getId() : 0));
			return;
		}

		if (!template.isNpcAllowed(-1) && ((npc == null) || !template.isNpcAllowed(npc.getId())))
		{
			if (player.isGM())
			{
				player.sendMessage("Multisell " + listId +
				                   " is restricted. Under current conditions cannot be used. Only GMs are allowed to use it.");
			}
			else
			{
				LOGGER.Warn(GetType().Name + ": " + player + " attempted to open multisell " + listId + " from npc " +
				            npc + " which is not allowed!");
				return;
			}
		}

		// Check if ingredient/product multipliers are set, if not, set them to the template value.
		double ingredientMultiplier = (Double.IsNaN(ingredientMultiplierValue)
			? template.getIngredientMultiplier()
			: ingredientMultiplierValue);
		double productMultiplier = (Double.IsNaN(productMultiplierValue)
			? template.getProductMultiplier()
			: productMultiplierValue);
		PreparedMultisellListHolder list = new PreparedMultisellListHolder(template, inventoryOnly,
			player.getInventory(), npc, ingredientMultiplier, productMultiplier);
		int index = 0;
		do
		{
			// send list at least once even if size = 0
			player.sendPacket(new MultiSellListPacket(player, list, index, type));
			index += PAGE_SIZE;
		} while (index < list.getEntries().size());

		player.setMultiSell(list);
	}

	public void separateAndSend(int listId, Player player, Npc npc, bool inventoryOnly)
	{
		separateAndSend(listId, player, npc, inventoryOnly, Double.NaN, Double.NaN, 0);
	}
	
	private bool itemExists(ItemHolder holder)
	{
		SpecialItemType specialItem = (SpecialItemType)holder.getId();
		if (Enum.IsDefined(specialItem))
		{
			return true;
		}
		
		ItemTemplate template = ItemData.getInstance().getTemplate(holder.getId());
		return (template != null) && (template.isStackable() ? (holder.getCount() >= 1) : (holder.getCount() == 1));
	}
	
	public MultisellListHolder getMultisell(int id)
	{
		return _multisells.getOrDefault(id, null);
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