using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class CombinationItemsData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CombinationItemsData));
	private readonly List<CombinationItem> _items = new();
	
	protected CombinationItemsData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_items.Clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/CombinationItems.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("item").ForEach(loadElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _items.size() + " combinations.");
	}

	private void loadElement(XElement element)
	{
		CombinationItem item = new CombinationItem(element);

		element.Elements("reward").ForEach(el =>
		{
			int id = el.Attribute("id").GetInt32();
			int count = el.Attribute("count").GetInt32(1);
			int enchant = el.Attribute("enchant").GetInt32(0);
			CombinationItemType type = el.Attribute("type").GetEnum<CombinationItemType>();
			item.addReward(new CombinationItemReward(id, count, type, enchant));
			if (ItemData.getInstance().getTemplate(id) == null)
			{
				LOGGER.Error(GetType().Name + ": Could not find item with id " + id);
			}
		});

		_items.add(item);
	}

	public int getLoadedElementsCount()
	{
		return _items.size();
	}
	
	public List<CombinationItem> getItems()
	{
		return _items;
	}
	
	public CombinationItem getItemsBySlots(int firstSlot, int enchantOne, int secondSlot, int enchantTwo)
	{
		foreach (CombinationItem item in _items)
		{
			if ((item.getItemOne() == firstSlot) && (item.getItemTwo() == secondSlot) && (item.getEnchantOne() == enchantOne) && (item.getEnchantTwo() == enchantTwo))
			{
				return item;
			}
		}
		return null;
	}
	
	public List<CombinationItem> getItemsByFirstSlot(int id, int enchantOne)
	{
		List<CombinationItem> result = new();
		foreach (CombinationItem item in _items)
		{
			if ((item.getItemOne() == id) && (item.getEnchantOne() == enchantOne))
			{
				result.add(item);
			}
		}
		return result;
	}
	
	public static CombinationItemsData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CombinationItemsData INSTANCE = new();
	}
}