using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.BuyList;

public class Product
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Product));

	private readonly int _buyListId;
	private readonly ItemTemplate _item;
	private readonly long _price;
	private readonly ProductRestock? _restock;
	private readonly double _baseTax;
	private long _count;

	public Product(int buyListId, ItemTemplate item, long price, ProductRestock? restock, int baseTax)
	{
		ArgumentNullException.ThrowIfNull(item);
		_buyListId = buyListId;
		_item = item;
		_price = price < 0 ? item.getReferencePrice() : price;
		_restock = restock;
		_baseTax = baseTax / 100.0;
		if (_restock != null)
		{
			_count = _restock.Count;
		}
	}

	public ItemTemplate getItem()
	{
		return _item;
	}

	public int getItemId()
	{
		return _item.getId();
	}

	public long getPrice()
	{
		long price = _price;
		if (_item.getItemType() == EtcItemType.CASTLE_GUARD)
		{
			price = (long)(price * Config.Rates.RATE_SIEGE_GUARDS_PRICE);
		}

		return price;
	}

	public double getBaseTaxRate()
	{
		return _baseTax;
	}

	public ProductRestock? getRestock()
	{
		return _restock;
	}

	public long getCount()
	{
		long count = Interlocked.Read(ref _count);
		return count > 0 ? count : 0;
	}

	public void setCount(long currentCount)
	{
		Interlocked.Exchange(ref _count, currentCount);
	}

	public bool decreaseCount(long value)
	{
		if (_restock == null)
			return false;

		BuyListTaskManager.getInstance().add(this, DateTime.UtcNow + _restock.Delay);

		bool result = Interlocked.Add(ref _count, -value) >= 0;
		save();
		return result;
	}

	public bool hasLimitedStock()
	{
		return _restock != null;
	}

	public void restartRestockTask(DateTime nextRestockTime)
	{
		TimeSpan remainTime = nextRestockTime - DateTime.UtcNow;
		if (remainTime > TimeSpan.Zero)
		{
			BuyListTaskManager.getInstance().update(this, nextRestockTime);
		}
		else
		{
			restock();
		}
	}

	public void restock()
	{
		if (_restock != null)
			setCount(_restock.Count);

		save();
	}

	private void save()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int itemId = _item.getId();
			DbBuyList? record = ctx.BuyLists.SingleOrDefault(r => r.BuyListId == _buyListId && r.ItemId == itemId);
			if (record is null)
			{
				record = new DbBuyList()
				{
					BuyListId = _buyListId,
					ItemId = itemId
				};
				ctx.BuyLists.Add(record);
			}

			record.Count = getCount();
			record.NextRestockTime = BuyListTaskManager.getInstance().getRestockDelay(this) ?? DateTime.UtcNow;

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("Failed to save Product buylist_id:" + _buyListId + " item_id=" + _item.getId() + ": " + e);
		}
	}
}