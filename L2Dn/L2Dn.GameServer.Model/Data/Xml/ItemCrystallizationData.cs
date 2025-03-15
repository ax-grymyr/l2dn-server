using System.Collections.Immutable;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid, Index
 */
public sealed class ItemCrystallizationData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemCrystallizationData));

    // index is CrystalType
    private readonly ImmutableArray<Map<CrystallizationType, List<ItemChanceHolder>>> _crystallizationTemplates =
        Enumerable.Range(0, EnumUtil.GetMaxValue<CrystalType>().ToInt32()).
            Select(_ => new Map<CrystallizationType, List<ItemChanceHolder>>()).ToImmutableArray();

	private readonly Map<int, CrystallizationDataHolder> _items = new();

	private RewardItemsOnFailure _weaponDestroyGroup = new();
	private RewardItemsOnFailure _armorDestroyGroup = new();

    private ItemCrystallizationData()
	{
        load();
	}

	public void load()
    {
		_crystallizationTemplates.ForEach(map => map.Clear());
		_items.Clear();

		_weaponDestroyGroup = new RewardItemsOnFailure();
		_armorDestroyGroup = new RewardItemsOnFailure();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "CrystallizableItems.xml");
		document.Elements("list").Elements("templates").Elements("crystallizable_template").ForEach(parseTemplate);
		document.Elements("list").Elements("items").Elements("crystallizable_item").ForEach(parseItem);
		document.Elements("list").Elements("itemsOnEnchantFailure").ForEach(el =>
		{
			el.Elements("armor").ForEach(e => _armorDestroyGroup = getFormedHolder(e));
			el.Elements("weapon").ForEach(e => _weaponDestroyGroup = getFormedHolder(e));
		});

        int crystallizationTemplateCount = _crystallizationTemplates.Sum(map => map.Sum(entry => entry.Value.Count));
		if (crystallizationTemplateCount > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + crystallizationTemplateCount + " crystallization templates.");
		}
		if (_items.Count > 0)
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _items.Count + " pre-defined crystallizable items.");
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

	private void parseTemplate(XElement element)
	{
		CrystalType crystalType = element.Attribute("crystalType").GetEnum<CrystalType>();
		CrystallizationType crystallizationType =
			element.Attribute("crystallizationType").GetEnum<CrystallizationType>();

		List<ItemChanceHolder> crystallizeRewards = [];
		element.Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
			long itemCount = el.GetAttributeValueAsInt64("count");
			double itemChance = el.GetAttributeValueAsDouble("chance");
			crystallizeRewards.Add(new ItemChanceHolder(itemId, itemChance, itemCount));
		});

		_crystallizationTemplates[(int)crystalType].put(crystallizationType, crystallizeRewards);
	}

	private void parseItem(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		List<ItemChanceHolder> crystallizeRewards = [];

		element.Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
			long itemCount = el.GetAttributeValueAsInt64("count");
			double itemChance = el.GetAttributeValueAsDouble("chance");
			crystallizeRewards.Add(new ItemChanceHolder(itemId, itemChance, itemCount));
		});

		_items.put(id, new CrystallizationDataHolder(id, crystallizeRewards));
	}

	private static List<ItemChanceHolder> CalculateCrystallizeRewards(ItemTemplate item, List<ItemChanceHolder> crystallizeRewards)
	{
		List<ItemChanceHolder> rewards = new();
		foreach (ItemChanceHolder reward in crystallizeRewards)
		{
			double chance = reward.getChance() * item.getCrystalCount();
			long count = reward.getCount();
			if (chance > 100.0)
			{
				double countMul = Math.Ceiling(chance / 100.0);
				chance /= countMul;
				count = (long)(count * countMul);
			}

			rewards.Add(new ItemChanceHolder(reward.getId(), chance, count));
		}

		return rewards;
	}

	private void generateCrystallizationData()
	{
		int previousCount = _items.Count;
		foreach (ItemTemplate item in ItemData.getInstance().getAllItems())
		{
			// Check if the data has not been generated.
			if (item is Weapon or Armor && item.isCrystallizable() && !_items.ContainsKey(item.getId()))
            {
                List<ItemChanceHolder>? holder = _crystallizationTemplates[(int)item.getCrystalType()].
                    get(item is Weapon ? CrystallizationType.WEAPON : CrystallizationType.ARMOR);

				if (holder != null)
				{
					_items.put(item.getId(), new CrystallizationDataHolder(item.getId(), CalculateCrystallizeRewards(item, holder)));
				}
			}
		}

		int generated = _items.Count - previousCount;
		if (generated > 0)
		{
			LOGGER.Info(GetType().Name + ": Generated " + generated + " crystallizable items from templates.");
		}
	}

	public List<ItemChanceHolder>? getCrystallizationTemplate(CrystalType crystalType, CrystallizationType crystallizationType)
	{
		return _crystallizationTemplates[(int)crystalType].get(crystallizationType);
	}

	/**
	 * @param itemId
	 * @return {@code CrystallizationData} for unenchanted items (enchanted items just have different crystal count, but same rewards),<br>
	 *         or {@code null} if there is no such data registered.
	 */
	public CrystallizationDataHolder? getCrystallizationData(int itemId)
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
		CrystallizationDataHolder? data = getCrystallizationData(item.getId());
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
				result.Add(new ItemChanceHolder(crystalItemId, 100, item.getCrystalCount()));
			}

			result.AddRange(items);
		}
		else
		{
			// Add basic crystal reward.
			result.Add(new ItemChanceHolder(crystalItemId, 100, item.getCrystalCount()));
		}

		return result;
	}

	private RewardItemsOnFailure getFormedHolder(XElement element)
	{
		RewardItemsOnFailure holder = new RewardItemsOnFailure();
		element.Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
			int enchantLevel = el.GetAttributeValueAsInt32("enchant");
			double chance = el.GetAttributeValueAsDouble("chance");
			foreach (CrystalType grade in EnumUtil.GetValues<CrystalType>())
			{
				long count = el.Attribute("amount" + grade).GetInt64(-1);
				if (count <= 0)
					continue;

				holder.addItemToHolder(itemId, grade, enchantLevel, count, chance);
			}
		});

		return holder;
	}

	public ItemChanceHolder? getItemOnDestroy(Player player, Item item)
	{
		if (player == null || item == null)
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