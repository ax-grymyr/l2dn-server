using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Berezkin Nikolay
 */
public class CollectionData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CollectionData));
	
	private static readonly Map<int, CollectionDataHolder> _collections = new();
	private static readonly Map<int, List<CollectionDataHolder>> _collectionsByTabId = new();
	
	protected CollectionData()
	{
		load();
	}
	
	public void load()
	{
		_collections.clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/CollectionData.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Root?.Elements("collection").ForEach(loadElement);
		
		if (!_collections.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _collections.size() + " collections.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
		}
	}

	private void loadElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		int optionId = element.Attribute("optionId").GetInt32();
		int category = element.Attribute("category").GetInt32();
		int completeCount = element.Attribute("completeCount").GetInt32();
		List<ItemEnchantHolder> items = new();
		
		element.Elements("item").ForEach(el =>
		{
			int itemId = el.Attribute("id").GetInt32();
			long itemCount = el.Attribute("count").GetInt64(1);
			int itemEnchantLevel = el.Attribute("enchantLevel").GetInt32(0);
			ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
			if (item == null)
			{
				LOGGER.Error(GetType().Name + ": Item template null for itemId: " + itemId + " collection item: " + id);
				return;
			}

			items.add(new ItemEnchantHolder(itemId, itemCount, itemEnchantLevel));
		});

		CollectionDataHolder template = new CollectionDataHolder(id, optionId, category, completeCount, items);
		_collections.put(id, template);
		_collectionsByTabId.computeIfAbsent(template.getCategory(), list => new()).add(template);
	}

	public CollectionDataHolder getCollection(int id)
	{
		return _collections.get(id);
	}
	
	public List<CollectionDataHolder> getCollectionsByTabId(int tabId)
	{
		if (_collectionsByTabId.containsKey(tabId))
		{
			return _collectionsByTabId.get(tabId);
		}
		return new();
	}
	
	public ICollection<CollectionDataHolder> getCollections()
	{
		return _collections.values();
	}
	
	public static CollectionData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CollectionData INSTANCE = new();
	}
}