using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class EnchantItemGroupsData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnchantItemGroupsData));
	
	private readonly Map<String, EnchantItemGroup> _itemGroups = new();
	private readonly Map<int, EnchantScrollGroup> _scrollGroups = new();
	private int _maxWeaponEnchant = 0;
	private int _maxArmorEnchant = 0;
	private int _maxAccessoryEnchant = 0;
	
	protected EnchantItemGroupsData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_itemGroups.clear();
		_scrollGroups.clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/EnchantItemGroups.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("enchantRateGroup").ForEach(parseEnchantRateGroup);
		document.Elements("list").Elements("enchantScrollGroup").ForEach(parseEnchantScrollGroup);
		
		// In case there is no accessories group set.
		if (_maxAccessoryEnchant == 0)
		{
			_maxAccessoryEnchant = _maxArmorEnchant;
		}
		
		// Max enchant values are set to current max enchant + 1.
		_maxWeaponEnchant += 1;
		_maxArmorEnchant += 1;
		_maxAccessoryEnchant += 1;
		
		LOGGER.Info(GetType().Name + ": Loaded " + _itemGroups.size() + " item group templates.");
		LOGGER.Info(GetType().Name + ": Loaded " + _scrollGroups.size() + " scroll group templates.");
		
		if (Config.OVER_ENCHANT_PROTECTION)
		{
			LOGGER.Info(GetType().Name + ": Max weapon enchant is set to " + _maxWeaponEnchant + ".");
			LOGGER.Info(GetType().Name + ": Max armor enchant is set to " + _maxArmorEnchant + ".");
			LOGGER.Info(GetType().Name + ": Max accessory enchant is set to " + _maxAccessoryEnchant + ".");
		}
	}

	private void parseEnchantRateGroup(XElement element)
	{
		string name = element.Attribute("name").GetString();
		EnchantItemGroup group = new EnchantItemGroup(name);
		
		element.Elements("current").ForEach(currentElement =>
		{
			string range = currentElement.Attribute("enchant").GetString();
			double chance = currentElement.Attribute("chance").GetDouble();
			int min = -1;
			int max = 0;
			if (range.contains("-"))
			{
				String[] split = range.Split("-");
				if ((split.Length == 2) && Util.isDigit(split[0]) && Util.isDigit(split[1]))
				{
					min = int.Parse(split[0]);
					max = int.Parse(split[1]);
				}
			}
			else if (Util.isDigit(range))
			{
				min = int.Parse(range);
				max = min;
			}

			if ((min > -1) && (max > -1))
			{
				group.addChance(new RangeChanceHolder(min, max, chance));
			}

			// Try to get a generic max value.
			if (chance > 0)
			{
				if (name.contains("WEAPON"))
				{
					if (_maxWeaponEnchant < max)
					{
						_maxWeaponEnchant = max;
					}
				}
				else if (name.contains("ACCESSORIES") || name.contains("RING") || name.contains("EARRING") ||
				         name.contains("NECK"))
				{
					if (_maxAccessoryEnchant < max)
					{
						_maxAccessoryEnchant = max;
					}
				}
				else if (_maxArmorEnchant < max)
				{
					_maxArmorEnchant = max;
				}
			}
		});

		_itemGroups.put(name, group);
	}

	private void parseEnchantScrollGroup(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		EnchantScrollGroup group = new EnchantScrollGroup(id);
		
		element.Elements("enchantRate").ForEach(enchantElement =>
		{
			string name = enchantElement.Attribute("group").GetString();
			EnchantRateItem rateGroup = new EnchantRateItem(name);
			
			enchantElement.Elements("item").ForEach(itemElement =>
			{
				if (itemElement.Attribute("slot") != null)
				{
					rateGroup.addSlot(ItemData.SLOTS.get(itemElement.Attribute("slot").GetString()));
				}
				if (itemElement.Attribute("magicWeapon") != null)
				{
					rateGroup.setMagicWeapon(itemElement.Attribute("magicWeapon").GetBoolean());
				}
				if (itemElement.Attribute("itemId") != null)
				{
					rateGroup.addItemId(itemElement.Attribute("itemId").GetInt32());
				}
			});

			group.addRateGroup(rateGroup);
		});

		_scrollGroups.put(id, group);
	}

	public EnchantItemGroup getItemGroup(ItemTemplate item, int scrollGroup)
	{
		EnchantScrollGroup group = _scrollGroups.get(scrollGroup);
		EnchantRateItem rateGroup = group.getRateGroup(item);
		return rateGroup != null ? _itemGroups.get(rateGroup.getName()) : null;
	}
	
	public EnchantItemGroup getItemGroup(String name)
	{
		return _itemGroups.get(name);
	}
	
	public EnchantScrollGroup getScrollGroup(int id)
	{
		return _scrollGroups.get(id);
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