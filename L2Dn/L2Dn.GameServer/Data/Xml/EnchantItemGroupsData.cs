using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Utilities;
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
		parseDatapackFile("data/EnchantItemGroups.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _itemGroups.size() + " item group templates.");
		LOGGER.Info(GetType().Name + ": Loaded " + _scrollGroups.size() + " scroll group templates.");
		
		if (Config.OVER_ENCHANT_PROTECTION)
		{
			LOGGER.Info(GetType().Name + ": Max weapon enchant is set to " + _maxWeaponEnchant + ".");
			LOGGER.Info(GetType().Name + ": Max armor enchant is set to " + _maxArmorEnchant + ".");
			LOGGER.Info(GetType().Name + ": Max accessory enchant is set to " + _maxAccessoryEnchant + ".");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("enchantRateGroup".equalsIgnoreCase(d.getNodeName()))
					{
						String name = parseString(d.getAttributes(), "name");
						EnchantItemGroup group = new EnchantItemGroup(name);
						for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
						{
							if ("current".equalsIgnoreCase(cd.getNodeName()))
							{
								String range = parseString(cd.getAttributes(), "enchant");
								double chance = parseDouble(cd.getAttributes(), "chance");
								int min = -1;
								int max = 0;
								if (range.contains("-"))
								{
									String[] split = range.split("-");
									if ((split.length == 2) && Util.isDigit(split[0]) && Util.isDigit(split[1]))
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
									else if (name.contains("ACCESSORIES") || name.contains("RING") || name.contains("EARRING") || name.contains("NECK"))
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
							}
						}
						_itemGroups.put(name, group);
					}
					else if ("enchantScrollGroup".equals(d.getNodeName()))
					{
						int id = parseInteger(d.getAttributes(), "id");
						EnchantScrollGroup group = new EnchantScrollGroup(id);
						for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
						{
							if ("enchantRate".equalsIgnoreCase(cd.getNodeName()))
							{
								EnchantRateItem rateGroup = new EnchantRateItem(parseString(cd.getAttributes(), "group"));
								for (Node z = cd.getFirstChild(); z != null; z = z.getNextSibling())
								{
									if ("item".equals(z.getNodeName()))
									{
										NamedNodeMap attrs = z.getAttributes();
										if (attrs.getNamedItem("slot") != null)
										{
											rateGroup.addSlot(ItemData.SLOTS.get(parseString(attrs, "slot")));
										}
										if (attrs.getNamedItem("magicWeapon") != null)
										{
											rateGroup.setMagicWeapon(parseBoolean(attrs, "magicWeapon"));
										}
										if (attrs.getNamedItem("itemId") != null)
										{
											rateGroup.addItemId(parseInteger(attrs, "itemId"));
										}
									}
								}
								group.addRateGroup(rateGroup);
							}
						}
						_scrollGroups.put(id, group);
					}
				}
			}
		}
		
		// In case there is no accessories group set.
		if (_maxAccessoryEnchant == 0)
		{
			_maxAccessoryEnchant = _maxArmorEnchant;
		}
		
		// Max enchant values are set to current max enchant + 1.
		_maxWeaponEnchant += 1;
		_maxArmorEnchant += 1;
		_maxAccessoryEnchant += 1;
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