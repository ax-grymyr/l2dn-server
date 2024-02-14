using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Options;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
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
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/EnchantItemOptions.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("item").ForEach(parseItem);

		int optionCount = _data.values().SelectMany(v => v.values()).Count();
		LOGGER.Info(GetType().Name + ": Loaded " + _data.size() + " items and " + optionCount + " options.");
	}

	private void parseItem(XElement element)
	{
		int itemId = element.Attribute("id").GetInt32();
		ItemTemplate template = ItemData.getInstance().getTemplate(itemId);
		if (template == null)
		{
			LOGGER.Warn(GetType().Name + ": Could not find item template for id " + itemId);
			return;
		}

		element.Elements("options").ForEach(optionsElement =>
		{
			int level = optionsElement.Attribute("level").GetInt32();
			EnchantOptions op = new EnchantOptions(level);
			for (byte i = 0; i < 3; i++)
			{
				XAttribute? optionAttribute = optionsElement.Attribute("option" + (i + 1));
				if (optionAttribute != null && Util.isDigit(optionAttribute.Value))
				{
					int id = (int)optionAttribute;
					if (OptionData.getInstance().getOptions(id) == null)
					{
						LOGGER.Warn(GetType().Name + ": Could not find option " + id + " for item " + template);
						return;
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
		});
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