using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid, Mobius
 */
public class EnchantItemOptionsData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(EnchantItemOptionsData));
	
	private readonly Map<int, Map<int, EnchantOptions>> _data = new();
	
	protected EnchantItemOptionsData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_data.clear();
		parseDatapackFile("data/EnchantItemOptions.xml");
	}
	
	public void parseDocument(Document doc, File f)
	{
		int counter = 0;
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				ITEM: for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("item".equalsIgnoreCase(d.getNodeName()))
					{
						int itemId = parseInteger(d.getAttributes(), "id");
						ItemTemplate template = ItemData.getInstance().getTemplate(itemId);
						if (template == null)
						{
							LOGGER.Warn(GetType().Name + ": Could not find item template for id " + itemId);
							continue ITEM;
						}
						for (Node cd = d.getFirstChild(); cd != null; cd = cd.getNextSibling())
						{
							if ("options".equalsIgnoreCase(cd.getNodeName()))
							{
								EnchantOptions op = new EnchantOptions(parseInteger(cd.getAttributes(), "level"));
								for (byte i = 0; i < 3; i++)
								{
									Node att = cd.getAttributes().getNamedItem("option" + (i + 1));
									if ((att != null) && Util.isDigit(att.getNodeValue()))
									{
										int id = parseInteger(att);
										if (OptionData.getInstance().getOptions(id) == null)
										{
											LOGGER.Warn(GetType().Name + ": Could not find option " + id + " for item " + template);
											continue ITEM;
										}
										
										Map<int, EnchantOptions> data = _data.get(itemId);
										if (data == null)
										{
											data = new();
											_data.put(itemId, data);
										}
										if (!data.containsKey(op.getLevel()))
										{
											data.put(op.getLevel(), op);
										}
										
										op.setOption(i, id);
									}
								}
								counter++;
							}
						}
					}
				}
			}
		}
		LOGGER.Info(GetType().Name + ": Loaded " + _data.size() + " items and " + counter + " options.");
	}
	
	/**
	 * @param itemId
	 * @return if specified item id has available enchant effects.
	 */
	public bool hasOptions(int itemId)
	{
		return _data.containsKey(itemId);
	}
	
	/**
	 * @param itemId
	 * @param enchantLevel
	 * @return enchant effects information.
	 */
	public EnchantOptions getOptions(int itemId, int enchantLevel)
	{
		if (!_data.containsKey(itemId) || !_data.get(itemId).containsKey(enchantLevel))
		{
			return null;
		}
		return _data.get(itemId).get(enchantLevel);
	}
	
	/**
	 * @param item
	 * @return enchant effects information.
	 */
	public EnchantOptions getOptions(Item item)
	{
		return item != null ? getOptions(item.getId(), item.getEnchantLevel()) : null;
	}
	
	/**
	 * Gets the single instance of EnchantOptionsData.
	 * @return single instance of EnchantOptionsData
	 */
	public static EnchantItemOptionsData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EnchantItemOptionsData INSTANCE = new();
	}
}