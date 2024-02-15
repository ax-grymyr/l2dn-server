using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads item enchant data.
 * @author UnAfraid
 */
public class EnchantItemData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnchantItemData));
	
	private readonly Map<int, EnchantScroll> _scrolls = new();
	private readonly Map<int, EnchantSupportItem> _supports = new();
	
	/**
	 * Instantiates a new enchant item data.
	 */
	public EnchantItemData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_scrolls.clear();
		_supports.clear();
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "EnchantItemData.xml");
		document.Elements("list").Elements("enchant").ForEach(parseEnchant);
		document.Elements("list").Elements("support").ForEach(parseSupport);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _scrolls.size() + " enchant scrolls.");
		LOGGER.Info(GetType().Name + ": Loaded " + _supports.size() + " support items.");
	}

	private void parseEnchant(XElement element)
	{
		try
		{
			EnchantScroll item = new EnchantScroll(element);
			element.Elements("item").ForEach(itemElement =>
			{
				int id = itemElement.Attribute("id").GetInt32();
				int altScrollGroupId = itemElement.Attribute("altScrollGroupId").GetInt32(-1);
				item.addItem(id, altScrollGroupId);
			});

			_scrolls.put(item.getId(), item);
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Unexistent enchant scroll or wrong data: " +
			             element.Attribute("id").GetInt32() +
			             " defined in enchant data!: " + e);
		}
	}

	private void parseSupport(XElement element)
	{
		try
		{
			EnchantSupportItem item = new EnchantSupportItem(element);
			_supports.put(item.getId(), item);
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Unexistent enchant support item or wrong data: " +
			            element.Attribute("id").GetInt32() + " defined in enchant data!: " + e);
		}
	}
	
	public ICollection<EnchantScroll> getScrolls()
	{
		return _scrolls.values();
	}
	
	/**
	 * Gets the enchant scroll.
	 * @param item the scroll item
	 * @return enchant template for scroll
	 */
	public EnchantScroll getEnchantScroll(Item item)
	{
		if (item == null)
		{
			return null;
		}
		return _scrolls.get(item.getId());
	}
	
	/**
	 * Gets the support item.
	 * @param item the support item
	 * @return enchant template for support item
	 */
	public EnchantSupportItem getSupportItem(Item item)
	{
		if (item == null)
		{
			return null;
		}
		return _supports.get(item.getId());
	}
	
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