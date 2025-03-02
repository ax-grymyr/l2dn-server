using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// Loads item enchant data.
/// </summary>
public sealed class EnchantItemData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnchantItemData));

	private static readonly Set<EtcItemType> _enchantTypes =
	[
		EtcItemType.ENCHT_ATTR_ANCIENT_CRYSTAL_ENCHANT_AM,
		EtcItemType.ENCHT_ATTR_ANCIENT_CRYSTAL_ENCHANT_WP,
		EtcItemType.BLESS_ENCHT_AM,
		EtcItemType.BLESS_ENCHT_AM_DOWN,
		EtcItemType.BLESS_ENCHT_WP,
		EtcItemType.ENCHT_AM,
		EtcItemType.ENCHT_WP,
		EtcItemType.GIANT_ENCHT_AM,
		EtcItemType.GIANT_ENCHT_WP,
		EtcItemType.ENCHT_ATTR_INC_PROP_ENCHT_AM,
		EtcItemType.ENCHT_ATTR_INC_PROP_ENCHT_WP,
		EtcItemType.GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM,
		EtcItemType.GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP,
		EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_AM,
		EtcItemType.BLESSED_ENCHT_ATTR_INC_PROP_ENCHT_WP,
		EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_AM,
		EtcItemType.BLESSED_GIANT_ENCHT_ATTR_INC_PROP_ENCHT_WP,
		EtcItemType.CURSED_ENCHT_AM,
		EtcItemType.CURSED_ENCHT_WP,
	];

	private FrozenDictionary<int, EnchantScroll> _scrolls = FrozenDictionary<int, EnchantScroll>.Empty;
	private FrozenDictionary<int, EnchantSupportItem> _supports = FrozenDictionary<int, EnchantSupportItem>.Empty;

	private EnchantItemData()
	{
		load();
	}

	public void load()
	{
		Dictionary<int, EnchantScroll> scrolls = [];
		Dictionary<int, EnchantSupportItem> supports = [];

		// TODO: xsd does not correspond to the DTO (support element is missing)
		XmlEnchantItemData document = LoadXmlDocument<XmlEnchantItemData>(DataFileLocation.Data, "EnchantItemData.xml");
		foreach (XmlEnchantScroll xmlEnchantScroll in document.EnchantScrolls)
		{
			int itemId = xmlEnchantScroll.Id;
			ItemTemplate? itemTemplate = ItemData.getInstance().getTemplate(itemId);
			if (itemTemplate == null)
			{
				_logger.Error(GetType().Name + ": Item id " + itemId + " not found in enchant data.");
				continue;
			}

			if (!itemTemplate.getItemType().IsEtcItem() ||
			    !_enchantTypes.Contains(itemTemplate.getItemType().AsEtcItemType()))
			{
				_logger.Error(GetType().Name + ": Invalid item id " + itemId + " in enchant data.");
				continue;
			}

			Dictionary<int, int> items = [];
			foreach (XmlEnchantScrollItem xmlEnchantScrollItem in xmlEnchantScroll.Items)
			{
				int id = xmlEnchantScrollItem.Id;
				int scrollGroupId = xmlEnchantScrollItem.AltScrollGroupId;
				if (!items.TryAdd(id, scrollGroupId > -1 ? scrollGroupId : xmlEnchantScroll.ScrollGroupId))
					_logger.Error(GetType().Name + ": Duplicated item id " + id + " in enchant data.");
			}

			EnchantScroll item = new(xmlEnchantScroll, itemTemplate.getItemType(), items.ToFrozenDictionary());
			if (!scrolls.TryAdd(item.getId(), item))
				_logger.Error(GetType().Name + ": Duplicated item id " + itemId + " in enchant data.");
		}

		_scrolls = scrolls.ToFrozenDictionary();
		_supports = supports.ToFrozenDictionary();

		_logger.Info(GetType().Name + ": Loaded " + _scrolls.Count + " enchant scrolls.");
		_logger.Info(GetType().Name + ": Loaded " + _supports.Count + " support items.");
	}

	public ImmutableArray<EnchantScroll> getScrolls()
	{
		return _scrolls.Values;
	}

	/**
	 * Gets the enchant scroll.
	 * @param item the scroll item
	 * @return enchant template for scroll
	 */
	public EnchantScroll? getEnchantScroll(int itemId) => _scrolls.GetValueOrDefault(itemId);

	/**
	 * Gets the support item.
	 * @param item the support item
	 * @return enchant template for support item
	 */
	public EnchantSupportItem? getSupportItem(int itemId) => _supports.GetValueOrDefault(itemId);

	/**
	 * Gets the single instance of EnchantItemData.
	 * @return single instance of EnchantItemData
	 */
	public static EnchantItemData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly EnchantItemData INSTANCE = new();
	}
}