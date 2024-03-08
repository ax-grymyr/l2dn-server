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
public class CollectionData: DataReaderBase
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
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "CollectionData.xml");
		document.Elements("list").Elements("collection").ForEach(loadElement);
		
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
		int id = element.GetAttributeValueAsInt32("id");
		int optionId = element.GetAttributeValueAsInt32("optionId");
		int category = element.GetAttributeValueAsInt32("category");
		int completeCount = element.GetAttributeValueAsInt32("completeCount");
		List<ItemEnchantHolder> items = new();
		
		element.Elements("item").ForEach(el =>
		{
			int itemId = el.GetAttributeValueAsInt32("id");
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