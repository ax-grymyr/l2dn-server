using System.Collections.Frozen;
using System.Globalization;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class EnchantItemGroupsData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(EnchantItemGroupsData));

	private FrozenDictionary<string, EnchantItemGroup> _itemGroups = FrozenDictionary<string, EnchantItemGroup>.Empty;
	private FrozenDictionary<int, EnchantScrollGroup> _scrollGroups = FrozenDictionary<int, EnchantScrollGroup>.Empty;
	private int _maxWeaponEnchant;
	private int _maxArmorEnchant;
	private int _maxAccessoryEnchant;

	private EnchantItemGroupsData()
	{
		load();
	}

	public void load()
	{
		XmlEnchantItemGroupData document =
			LoadXmlDocument<XmlEnchantItemGroupData>(DataFileLocation.Data, "EnchantItemGroups.xml");

		int maxWeaponEnchant = 0;
		int maxArmorEnchant = 0;
		int maxAccessoryEnchant = 0;
		Dictionary<string, EnchantItemGroup> itemGroups = [];
		Dictionary<int, EnchantScrollGroup> scrollGroups = [];

		foreach (XmlEnchantRateGroup xmlEnchantRateGroup in document.EnchantRateGroups)
		{
			string name = xmlEnchantRateGroup.Name;
			EnchantItemGroup group = new(name);
			foreach (XmlEnchantRateGroupCurrent xmlEnchantRateGroupCurrent in xmlEnchantRateGroup.Chances)
			{
				string range = xmlEnchantRateGroupCurrent.Enchant;
				double chance = xmlEnchantRateGroupCurrent.Chance;
				if (!ParseIntRange(xmlEnchantRateGroupCurrent.Enchant, out int min, out int max))
				{
					_logger.Error(GetType().Name + $": Invalid range {range}.");
					continue;
				}

				group.addChance(new RangeChanceHolder(min, max, chance));

				// Try to get a generic max value.
				if (chance > 0)
				{
					if (name.Contains("WEAPON", StringComparison.OrdinalIgnoreCase))
					{
						if (maxWeaponEnchant < max)
							maxWeaponEnchant = max;
					}
					else if (name.Contains("ACCESSORIES", StringComparison.OrdinalIgnoreCase) ||
					         name.Contains("RING", StringComparison.OrdinalIgnoreCase) ||
					         name.Contains("EARRING", StringComparison.OrdinalIgnoreCase) ||
					         name.Contains("NECK", StringComparison.OrdinalIgnoreCase))
					{
						if (maxAccessoryEnchant < max)
							maxAccessoryEnchant = max;
					}
					else if (maxArmorEnchant < max)
						maxArmorEnchant = max;
				}
			}

			if (!itemGroups.TryAdd(name, group))
				_logger.Error(GetType().Name + $": Duplicated group {name}.");
		}

		foreach (XmlEnchantScrollGroup xmlEnchantScrollGroup in document.EnchantScrollGroups)
		{
			int id = xmlEnchantScrollGroup.Id;
			EnchantScrollGroup group = new(id);
			foreach (XmlEnchantScrollGroupRate xmlEnchantScrollGroupRate in xmlEnchantScrollGroup.EnchantRates)
			{
				string name = xmlEnchantScrollGroupRate.Group;
				EnchantRateItem rateGroup = new(name);
				foreach (XmlEnchantScrollGroupRateItem xmlEnchantScrollGroupRateItem in xmlEnchantScrollGroupRate.Items)
				{
					if (!string.IsNullOrEmpty(xmlEnchantScrollGroupRateItem.Slot))
						rateGroup.addSlot(ItemData._slotNameMap[xmlEnchantScrollGroupRateItem.Slot]);

					rateGroup.setMagicWeapon(xmlEnchantScrollGroupRateItem.MagicWeapon);

					if (xmlEnchantScrollGroupRateItem.ItemId != 0)
						rateGroup.addItemId(xmlEnchantScrollGroupRateItem.ItemId);
				}

				group.addRateGroup(rateGroup);
			}

			if (!scrollGroups.TryAdd(id, group))
				_logger.Error(GetType().Name + $": Duplicated scroll group id {id}.");
		}

		// In case there is no accessories group set.
		if (maxAccessoryEnchant == 0)
			maxAccessoryEnchant = maxArmorEnchant;

		// Max enchant values are set to current max enchant + 1.
		maxWeaponEnchant += 1;
		maxArmorEnchant += 1;
		maxAccessoryEnchant += 1;

		_itemGroups = itemGroups.ToFrozenDictionary();
		_scrollGroups = scrollGroups.ToFrozenDictionary();
		_maxWeaponEnchant = maxWeaponEnchant;
		_maxArmorEnchant = maxArmorEnchant;
		_maxAccessoryEnchant = maxAccessoryEnchant;

		_logger.Info(GetType().Name + ": Loaded " + _itemGroups.Count + " item group templates.");
		_logger.Info(GetType().Name + ": Loaded " + _scrollGroups.Count + " scroll group templates.");

		if (Config.OVER_ENCHANT_PROTECTION)
		{
			_logger.Info(GetType().Name + ": Max weapon enchant is set to " + _maxWeaponEnchant + ".");
			_logger.Info(GetType().Name + ": Max armor enchant is set to " + _maxArmorEnchant + ".");
			_logger.Info(GetType().Name + ": Max accessory enchant is set to " + _maxAccessoryEnchant + ".");
		}
	}

	private static bool ParseIntRange(ReadOnlySpan<char> range, out int min, out int max)
	{
		min = max = 0;
		if (int.TryParse(range, CultureInfo.InvariantCulture, out min))
		{
			max = min;
			return true;
		}

		int separatorIndex = range.IndexOf('-');
		if (separatorIndex < 0)
			return false;

		return int.TryParse(range[..separatorIndex], CultureInfo.InvariantCulture, out min) &&
			int.TryParse(range[(separatorIndex + 1)..], CultureInfo.InvariantCulture, out max);
	}

	public EnchantItemGroup? getItemGroup(ItemTemplate item, int scrollGroup)
	{
		EnchantScrollGroup? group = _scrollGroups.GetValueOrDefault(scrollGroup);
		EnchantRateItem? rateGroup = group?.getRateGroup(item);
		return rateGroup != null ? _itemGroups.GetValueOrDefault(rateGroup.getName()) : null;
	}

	public EnchantItemGroup? getItemGroup(string name)
	{
		return _itemGroups.GetValueOrDefault(name);
	}

	public EnchantScrollGroup? getScrollGroup(int id)
	{
		return _scrollGroups.GetValueOrDefault(id);
	}

	public int getMaxWeaponEnchant()
	{
		return _maxWeaponEnchant;
	}

	public int getMaxArmorEnchant()
	{
		return _maxArmorEnchant;
	}

	public int getMaxAccessoryEnchant()
	{
		return _maxAccessoryEnchant;
	}

	public static EnchantItemGroupsData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly EnchantItemGroupsData INSTANCE = new();
	}
}