using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

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
			foreach (ItemTransactionHistory record in ctx.ItemTransactionHistory)
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
				if (!uniqueItemIds.containsKey(itemId))
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
					finalList.add(transaction);
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
			if (map.get(transaction.getItemId()) == null)
			{
				map.put(transaction.getItemId(),
					new ItemHistoryTransaction(transaction.getTransactionType(), transaction.getCount(),
						transaction.getPrice(), transaction.getItemId(), 0, false));
			}
			else
			{
				map.get(transaction.getItemId()).addCount(transaction.getCount());
			}
		}

		List<ItemHistoryTransaction> list = new();
		map.forEach(kvp => list.add(kvp.Value));
		list.Sort(new SortByQuantity());
		return list;
	}

	protected class SortByPrice: IComparer<ItemHistoryTransaction>
	{
		public int Compare(ItemHistoryTransaction a, ItemHistoryTransaction b)
		{
			return a.getPrice() > b.getPrice() ? -1 : a.getPrice() == b.getPrice() ? 0 : 1;
		}
	}

	protected class SortByQuantity: IComparer<ItemHistoryTransaction>
	{
		public int Compare(ItemHistoryTransaction a, ItemHistoryTransaction b)
		{
			return a.getCount() > b.getCount() ? -1 : a.getCount() == b.getCount() ? 0 : 1;
		}
	}

	protected class SortByDate: IComparer<ItemHistoryTransaction>
	{
		public int Compare(ItemHistoryTransaction a, ItemHistoryTransaction b)
		{
			return a.getTransactionDate() > b.getTransactionDate() ? -1 :
				a.getTransactionDate() == b.getTransactionDate() ? 0 : 1;
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

		public ItemHistoryTransaction(ItemTransactionHistory record)
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
				ctx.ItemTransactionHistory.Add(new ItemTransactionHistory()
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

		public override String ToString()
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