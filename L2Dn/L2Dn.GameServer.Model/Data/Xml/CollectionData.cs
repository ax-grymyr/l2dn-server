using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class CollectionData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(CollectionData));

	private static FrozenDictionary<int, CollectionDataHolder> _collections =
		FrozenDictionary<int, CollectionDataHolder>.Empty;

	private static FrozenDictionary<int, ImmutableArray<CollectionDataHolder>> _collectionsByTabId =
		FrozenDictionary<int, ImmutableArray<CollectionDataHolder>>.Empty;

	private CollectionData()
	{
		load();
	}

	public void load()
	{
		XmlCollectionData document = LoadXmlDocument<XmlCollectionData>(DataFileLocation.Data, "CollectionData.xml");
		Dictionary<int, CollectionDataHolder> collections = [];
		Dictionary<int, List<CollectionDataHolder>> collectionsByTabId = [];
		foreach (XmlCollection collection in document.Collections)
		{
			int id = collection.Id;
			List<ItemEnchantHolder> items = [];
			bool invalidCollection = false;
			foreach (XmlCollectionItem collectionItem in collection.Items)
			{
				int itemId = collectionItem.Id;
				ItemTemplate? item = ItemData.getInstance().getTemplate(itemId);
				if (item == null)
				{
					_logger.Error($"{GetType().Name}: Item template null for itemId: {itemId} collection item: {id}");
					invalidCollection = true;
					break;
				}

				items.Add(new ItemEnchantHolder(itemId, collectionItem.Count, collectionItem.EnchantLevel));
			}

			if (invalidCollection)
				continue;

			CollectionDataHolder template = new(id, collection.OptionId, collection.Category, collection.CompleteCount,
				items.ToImmutableArray());

			if (!collections.TryAdd(id, template))
			{
				_logger.Error($"{GetType().Name}: Duplicated collection id: {id}");
				continue;
			}

			if (!collectionsByTabId.TryGetValue(collection.Category, out List<CollectionDataHolder>? category))
				collectionsByTabId.Add(collection.Category, category = []);

			category.Add(template);
		}

		_collections = collections.ToFrozenDictionary();
		_collectionsByTabId = collectionsByTabId
			.Select(p => new KeyValuePair<int, ImmutableArray<CollectionDataHolder>>(p.Key, p.Value.ToImmutableArray()))
			.ToFrozenDictionary();

		if (!_collections.IsEmpty())
			_logger.Info(GetType().Name + ": Loaded " + _collections.Count + " collections.");
		else
			_logger.Info(GetType().Name + ": System is disabled.");
	}

	public CollectionDataHolder? getCollection(int id) => _collections.GetValueOrDefault(id);

	public ImmutableArray<CollectionDataHolder> getCollectionsByTabId(int tabId)
		=> _collectionsByTabId.TryGetValue(tabId, out ImmutableArray<CollectionDataHolder> collections)
			? collections
			: ImmutableArray<CollectionDataHolder>.Empty;

	public ImmutableArray<CollectionDataHolder> getCollections() => _collections.Values;

	public static CollectionData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly CollectionData INSTANCE = new();
	}
}