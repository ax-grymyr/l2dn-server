using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * Loads item enchant data.
 * @author UnAfraid
 */
public class EnchantItemData
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
		parseDatapackFile("data/EnchantItemData.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _scrolls.size() + " enchant scrolls.");
		LOGGER.Info(GetType().Name + ": Loaded " + _supports.size() + " support items.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		StatSet set;
		Node att;
		NamedNodeMap attrs;
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("enchant".equalsIgnoreCase(d.getNodeName()))
					{
						attrs = d.getAttributes();
						set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						
						try
						{
							EnchantScroll item = new EnchantScroll(set);
							for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
							{
								if ("item".equalsIgnoreCase(cd.getNodeName()))
								{
									item.addItem(parseInteger(cd.getAttributes(), "id"), parseInteger(cd.getAttributes(), "altScrollGroupId", -1));
								}
							}
							_scrolls.put(item.getId(), item);
						}
						catch (NullPointerException e)
						{
							LOGGER.Warn(GetType().Name + ": Unexistent enchant scroll: " + set.getString("id") + " defined in enchant data!");
						}
						catch (IllegalAccessError e)
						{
							LOGGER.Warn(GetType().Name + ": Wrong enchant scroll item type: " + set.getString("id") + " defined in enchant data!");
						}
					}
					else if ("support".equalsIgnoreCase(d.getNodeName()))
					{
						attrs = d.getAttributes();
						set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						
						try
						{
							EnchantSupportItem item = new EnchantSupportItem(set);
							_supports.put(item.getId(), item);
						}
						catch (NullPointerException e)
						{
							LOGGER.Warn(GetType().Name + ": Unexistent enchant support item: " + set.getString("id") + " defined in enchant data!");
						}
						catch (IllegalAccessError e)
						{
							LOGGER.Warn(GetType().Name + ": Wrong enchant support item type: " + set.getString("id") + " defined in enchant data!");
						}
					}
				}
			}
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