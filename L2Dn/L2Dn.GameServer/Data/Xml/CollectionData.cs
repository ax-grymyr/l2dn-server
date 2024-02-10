using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
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
		parseDatapackFile("data/CollectionData.xml");
		
		if (!_collections.isEmpty())
		{
			LOGGER.Info(GetType().Name + ": Loaded " + _collections.size() + " collections.");
		}
		else
		{
			LOGGER.Info(GetType().Name + ": System is disabled.");
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
					if ("collection".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						Node att;
						StatSet set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						
						int id = parseInteger(attrs, "id");
						int optionId = parseInteger(attrs, "optionId");
						int category = parseInteger(attrs, "category");
						int completeCount = parseInteger(attrs, "completeCount");
						List<ItemEnchantHolder> items = new();
						for (Node b = d.getFirstChild(); b != null; b = b.getNextSibling())
						{
							attrs = b.getAttributes();
							if ("item".equalsIgnoreCase(b.getNodeName()))
							{
								int itemId = parseInteger(attrs, "id");
								long itemCount = Parse(attrs, "count", 1L);
								int itemEnchantLevel = parseInteger(attrs, "enchantLevel", 0);
								ItemTemplate item = ItemData.getInstance().getTemplate(itemId);
								if (item == null)
								{
									LOGGER.Error(GetType().Name + ": Item template null for itemId: " + itemId + " collection item: " + id);
									continue;
								}
								items.add(new ItemEnchantHolder(itemId, itemCount, itemEnchantLevel));
							}
						}
						
						CollectionDataHolder template = new CollectionDataHolder(id, optionId, category, completeCount, items);
						_collections.put(id, template);
						_collectionsByTabId.computeIfAbsent(template.getCategory(), list => new()).add(template);
					}
				}
			}
		}
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