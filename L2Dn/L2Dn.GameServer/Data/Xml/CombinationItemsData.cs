using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.Utilities;
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
		parseDatapackFile("data/CombinationItems.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _items.size() + " combinations.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode => forEach(listNode, "item", itemNode =>
		{
			CombinationItem item = new CombinationItem(new StatSet(parseAttributes(itemNode)));
			forEach(itemNode, "reward", rewardNode =>
			{
				int id = parseInteger(rewardNode.getAttributes(), "id");
				int count = parseInteger(rewardNode.getAttributes(), "count", 1);
				int enchant = parseInteger(rewardNode.getAttributes(), "enchant", 0);
				CombinationItemType type = parseEnum(rewardNode.getAttributes(), CombinationItemType.class, "type");
				item.addReward(new CombinationItemReward(id, count, type, enchant));
				if (ItemData.getInstance().getTemplate(id) == null)
				{
					LOGGER.Info(GetType().Name + ": Could not find item with id " + id);
				}
			});
			_items.add(item);
		}));
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