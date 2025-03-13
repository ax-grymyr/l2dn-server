using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class PrivateStoreHistoryManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(PrivateStoreHistoryManager));
	private static readonly List<ItemHistoryTransaction> _items = new();

	public void registerTransaction(PrivateStoreType transactionType, Item item, long count, long price)
	{
		try
		{
			ItemHistoryTransaction historyItem = new ItemHistoryTransaction(transactionType, count, price, item);
			_items.Add(historyItem);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store history for item: " + item, e);
		}
	}

	public void restore()
	{
		_items.Clear();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (DbItemTransactionHistory record in ctx.ItemTransactionHistory)
			{
				ItemHistoryTransaction item = new ItemHistoryTransaction(record);
				_items.Add(item);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not restore history." + e);
		}

		LOGGER.Info(GetType().Name + ": Loaded " + _items.Count + " items history.");
	}

	public void reset()
	{
		_items.Clear();
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.ItemTransactionHistory.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not reset history." + e);
		}

		LOGGER.Info(GetType().Name + ": weekly reset.");
	}

	public List<ItemHistoryTransaction> getHistory()
	{
		return getHistory(false);
	}

	public List<ItemHistoryTransaction> getHistory(bool full)
	{
		if (!full)
		{
			List<ItemHistoryTransaction> tempList = new(_items);
			Map<int, int> uniqueItemIds = new();
			foreach (ItemHistoryTransaction transaction in tempList)
			{
				int itemId = transaction.getItemId();
				if (!uniqueItemIds.ContainsKey(itemId))
				{
					uniqueItemIds.put(itemId, 0);
				}
			}

			tempList.Sort(new SortByDate());

			List<ItemHistoryTransaction> finalList = new();
			foreach (ItemHistoryTransaction transaction in tempList)
			{
				int itemId = transaction.getItemId();
				if (uniqueItemIds.get(itemId) < Config.STORE_REVIEW_LIMIT)
				{
					finalList.Add(transaction);
					uniqueItemIds.put(itemId, uniqueItemIds.get(itemId) + 1);
				}
			}

			return finalList;
		}

		return _items;
	}

	public List<ItemHistoryTransaction> getTopHighestItem()
	{
		List<ItemHistoryTransaction> list = new(_items);
		list.Sort(new SortByPrice());
		return list;
	}

	public List<ItemHistoryTransaction> getTopMostItem()
	{
		Map<int, ItemHistoryTransaction> map = new();
        foreach (ItemHistoryTransaction transaction in _items)
        {
            ItemHistoryTransaction transactionInMap = map.GetOrAdd(transaction.getItemId(),
                static (_, tr) => new ItemHistoryTransaction(tr.getTransactionType(), tr.getCount(),
                    tr.getPrice(), tr.getItemId(), 0, false), transaction);

            transactionInMap.addCount(transaction.getCount());
        }

        List<ItemHistoryTransaction> list = [];
		map.ForEach(kvp => list.Add(kvp.Value));
		list.Sort(new SortByQuantity());
		return list;
	}

    private sealed class SortByPrice: IComparer<ItemHistoryTransaction>
	{
		public int Compare(ItemHistoryTransaction? a, ItemHistoryTransaction? b)
        {
            long aPrice = a?.getPrice() ?? 0;
            long bPrice = b?.getPrice() ?? 0;

            // TODO: descending order???
            return -aPrice.CompareTo(bPrice);
		}
	}

    private sealed class SortByQuantity: IComparer<ItemHistoryTransaction>
	{
        public int Compare(ItemHistoryTransaction? a, ItemHistoryTransaction? b)
        {
            long aCount = a?.getCount() ?? 0;
            long bCount = b?.getCount() ?? 0;

            // TODO: descending order???
            return -aCount.CompareTo(bCount);
        }
    }

    private sealed class SortByDate: IComparer<ItemHistoryTransaction>
	{
		public int Compare(ItemHistoryTransaction? a, ItemHistoryTransaction? b)
		{
            DateTime aDate = a?.getTransactionDate() ?? DateTime.MinValue;
            DateTime bDate = b?.getTransactionDate() ?? DateTime.MinValue;

            // TODO: descending order???
			return -aDate.CompareTo(bDate);
		}
	}

	public class ItemHistoryTransaction
	{
		private readonly DateTime _transactionDate;
		private readonly int _itemId;
		private readonly PrivateStoreType _transactionType;
		private readonly int _enchantLevel;
		private readonly long _price;
		private long _count;

		public ItemHistoryTransaction(DbItemTransactionHistory record)
		{
			_transactionDate = record.CreatedTime;
			_itemId = record.ItemId;
			_transactionType = record.TransactionType == 0 ? PrivateStoreType.SELL : PrivateStoreType.BUY;
			_enchantLevel = record.EnchantLevel;
			_price = record.Price;
			_count = record.Count;
		}

		public ItemHistoryTransaction(PrivateStoreType transactionType, long count, long price, Item item)
			: this(transactionType, count, price, item.getId(), item.getEnchantLevel(), true)
		{
		}

		public ItemHistoryTransaction(PrivateStoreType transactionType, long count, long price, int itemId,
			int enchantLevel, bool saveToDB)
		{
			_transactionDate = DateTime.UtcNow;
			_itemId = itemId;
			_transactionType = transactionType;
			_enchantLevel = enchantLevel;
			_price = price;
			_count = count;

			if (saveToDB)
			{
				storeInDB();
			}
		}

		public DateTime getTransactionDate()
		{
			return _transactionDate;
		}

		public PrivateStoreType getTransactionType()
		{
			return _transactionType;
		}

		public int getItemId()
		{
			return _itemId;
		}

		public int getEnchantLevel()
		{
			return _enchantLevel;
		}

		public long getPrice()
		{
			return _price;
		}

		public long getCount()
		{
			return _count;
		}

		public void addCount(long count)
		{
			_count += count;
		}

		private void storeInDB()
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.ItemTransactionHistory.Add(new DbItemTransactionHistory()
				{
					CreatedTime = _transactionDate,
					ItemId = _itemId,
					TransactionType = _transactionType == PrivateStoreType.SELL ? 0 : 1,
					EnchantLevel = _enchantLevel,
					Count = _count,
					Price = _price
				});

				ctx.SaveChanges();
			}
			catch (Exception e)
			{
				LOGGER.Error("Could not insert history item " + this + " into DB: Reason: " + e);
			}
		}

		public override string ToString()
		{
			return _transactionDate + "(" + _transactionType + ")" + "[" + _itemId + " +" + _enchantLevel + " c:" +
			       _count + " p:" + _price + " ]";
		}
	}

	public static PrivateStoreHistoryManager getInstance()
	{
		return PrivateStoreHistoryManager.SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly PrivateStoreHistoryManager INSTANCE = new PrivateStoreHistoryManager();
	}
}