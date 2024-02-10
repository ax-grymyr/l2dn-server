using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid, Index
 */
public class ItemCrystallizationData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemCrystallizationData));
	
	private readonly Map<CrystalType, Map<CrystallizationType, List<ItemChanceHolder>>> _crystallizationTemplates = new();
	private readonly Map<int, CrystallizationDataHolder> _items = new();
	
	private RewardItemsOnFailure _weaponDestroyGroup = new RewardItemsOnFailure();
	private RewardItemsOnFailure _armorDestroyGroup = new RewardItemsOnFailure();
	
	protected ItemCrystallizationData()
	{
		load();
	}
	
	public void load()
	{
		_crystallizationTemplates.clear();
		foreach (CrystalType crystalType in Enum.GetValues<CrystalType>())
		{
			_crystallizationTemplates.put(crystalType, new());
		}
		_items.clear();
		
		_weaponDestroyGroup = new RewardItemsOnFailure();
		_armorDestroyGroup = new RewardItemsOnFailure();
		
		parseDatapackFile("data/CrystallizableItems.xml");
		
		if (_crystallizationTemplates.size() > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _crystallizationTemplates.size() + " crystallization templates.");
		}
		if (_items.size() > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _items.size() + " pre-defined crystallizable items.");
		}
		
		// Generate remaining data.
		generateCrystallizationData();
		
		if (_weaponDestroyGroup.size() > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _weaponDestroyGroup.size() + " weapon enchant failure rewards.");
		}
		if (_armorDestroyGroup.size() > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _armorDestroyGroup.size() + " armor enchant failure rewards.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node o = n.getFirstChild(); o != null; o = o.getNextSibling())
				{
					if ("templates".equalsIgnoreCase(o.getNodeName()))
					{
						for (Node d = o.getFirstChild(); d != null; d = d.getNextSibling())
						{
							if ("crystallizable_template".equalsIgnoreCase(d.getNodeName()))
							{
								CrystalType crystalType = parseEnum(d.getAttributes(), CrystalType.class, "crystalType");
								CrystallizationType crystallizationType = parseEnum(d.getAttributes(), CrystallizationType.class, "crystallizationType");
								List<ItemChanceHolder> crystallizeRewards = new();
								for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
								{
									if ("item".equalsIgnoreCase(c.getNodeName()))
									{
										NamedNodeMap attrs = c.getAttributes();
										int itemId = parseInteger(attrs, "id");
										long itemCount = Parse(attrs, "count");
										double itemChance = parseDouble(attrs, "chance");
										crystallizeRewards.add(new ItemChanceHolder(itemId, itemChance, itemCount));
									}
								}
								
								_crystallizationTemplates.get(crystalType).put(crystallizationType, crystallizeRewards);
							}
						}
					}
					else if ("items".equalsIgnoreCase(o.getNodeName()))
					{
						for (Node d = o.getFirstChild(); d != null; d = d.getNextSibling())
						{
							if ("crystallizable_item".equalsIgnoreCase(d.getNodeName()))
							{
								int id = parseInteger(d.getAttributes(), "id");
								List<ItemChanceHolder> crystallizeRewards = new();
								for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
								{
									if ("item".equalsIgnoreCase(c.getNodeName()))
									{
										NamedNodeMap attrs = c.getAttributes();
										int itemId = parseInteger(attrs, "id");
										long itemCount = Parse(attrs, "count");
										double itemChance = parseDouble(attrs, "chance");
										crystallizeRewards.add(new ItemChanceHolder(itemId, itemChance, itemCount));
									}
								}
								_items.put(id, new CrystallizationDataHolder(id, crystallizeRewards));
							}
						}
					}
					else if ("itemsOnEnchantFailure".equals(o.getNodeName()))
					{
						for (Node d = o.getFirstChild(); d != null; d = d.getNextSibling())
						{
							if ("armor".equalsIgnoreCase(d.getNodeName()))
							{
								_armorDestroyGroup = getFormedHolder(d);
							}
							else if ("weapon".equalsIgnoreCase(d.getNodeName()))
							{
								_weaponDestroyGroup = getFormedHolder(d);
							}
						}
					}
				}
			}
		}
	}
	
	public int getLoadedCrystallizationTemplateCount()
	{
		return _crystallizationTemplates.size();
	}
	
	private List<ItemChanceHolder> calculateCrystallizeRewards(ItemTemplate item, List<ItemChanceHolder> crystallizeRewards)
	{
		if (crystallizeRewards == null)
		{
			return null;
		}
		
		List<ItemChanceHolder> rewards = new();
		foreach (ItemChanceHolder reward in crystallizeRewards)
		{
			double chance = reward.getChance() * item.getCrystalCount();
			long count = reward.getCount();
			if (chance > 100.0)
			{
				double countMul = Math.ceil(chance / 100.0);
				chance /= countMul;
				count *= countMul;
			}
			
			rewards.add(new ItemChanceHolder(reward.getId(), chance, count));
		}
		
		return rewards;
	}
	
	private void generateCrystallizationData()
	{
		int previousCount = _items.size();
		foreach (ItemTemplate item in ItemData.getInstance().getAllItems())
		{
			// Check if the data has not been generated.
			if (((item is Weapon) || (item is Armor)) && item.isCrystallizable() && !_items.containsKey(item.getId()))
			{
				List<ItemChanceHolder> holder = _crystallizationTemplates.get(item.getCrystalType()).get((item is Weapon) ? CrystallizationType.WEAPON : CrystallizationType.ARMOR);
				if (holder != null)
				{
					_items.put(item.getId(), new CrystallizationDataHolder(item.getId(), calculateCrystallizeRewards(item, holder)));
				}
			}
		}
		
		int generated = _items.size() - previousCount;
		if (generated > 0)
		{
			LOGGER.Info(GetType().Name + ": Generated " + generated + " crystallizable items from templates.");
		}
	}
	
	public List<ItemChanceHolder> getCrystallizationTemplate(CrystalType crystalType, CrystallizationType crystallizationType)
	{
		return _crystallizationTemplates.get(crystalType).get(crystallizationType);
	}
	
	/**
	 * @param itemId
	 * @return {@code CrystallizationData} for unenchanted items (enchanted items just have different crystal count, but same rewards),<br>
	 *         or {@code null} if there is no such data registered.
	 */
	public CrystallizationDataHolder getCrystallizationData(int itemId)
	{
		return _items.get(itemId);
	}
	
	/**
	 * @param item to calculate its worth in crystals.
	 * @return List of {@code ItemChanceHolder} for the rewards with altered crystal count.
	 */
	public List<ItemChanceHolder> getCrystallizationRewards(Item item)
	{
		List<ItemChanceHolder> result = new();
		int crystalItemId = item.getTemplate().getCrystalItemId();
		CrystallizationDataHolder data = getCrystallizationData(item.getId());
		if (data != null)
		{
			// If there are no crystals on the template, add such.
			bool found = false;
			List<ItemChanceHolder> items = data.getItems();
			foreach (ItemChanceHolder holder in items)
			{
				if (holder.getId() == crystalItemId)
				{
					found = true;
					break;
				}
			}
			if (!found)
			{
				result.add(new ItemChanceHolder(crystalItemId, 100, item.getCrystalCount()));
			}
			
			result.addAll(items);
		}
		else
		{
			// Add basic crystal reward.
			result.add(new ItemChanceHolder(crystalItemId, 100, item.getCrystalCount()));
		}
		
		return result;
	}
	
	private RewardItemsOnFailure getFormedHolder(Node node)
	{
		RewardItemsOnFailure holder = new RewardItemsOnFailure();
		for (Node z = node.getFirstChild(); z != null; z = z.getNextSibling())
		{
			if ("item".equals(z.getNodeName()))
			{
				StatSet failItems = new StatSet(parseAttributes(z));
				int itemId = failItems.getInt("id");
				int enchantLevel = failItems.getInt("enchant");
				double chance = failItems.getDouble("chance");
				foreach (CrystalType grade in Enum.GetValues<CrystalType>())
				{
					long count = failItems.getLong("amount" + grade.name(), int.MinValue);
					if (count == int.MinValue)
					{
						continue;
					}
					
					holder.addItemToHolder(itemId, grade, enchantLevel, count, chance);
				}
			}
		}
		return holder;
	}
	
	public ItemChanceHolder getItemOnDestroy(Player player, Item item)
	{
		if ((player == null) || (item == null))
		{
			return null;
		}
		
		RewardItemsOnFailure holder = item.isWeapon() ? _weaponDestroyGroup : _armorDestroyGroup;
		CrystalType grade = item.getTemplate().getCrystalTypePlus();
		if (holder.checkIfRewardUnavailable(grade, item.getEnchantLevel()))
		{
			return null;
		}
		
		return holder.getRewardItem(grade, item.getEnchantLevel());
	}
	
	/**
	 * Gets the single instance of ItemCrystalizationData.
	 * @return single instance of ItemCrystalizationData
	 */
	public static ItemCrystallizationData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemCrystallizationData INSTANCE = new();
	}
}