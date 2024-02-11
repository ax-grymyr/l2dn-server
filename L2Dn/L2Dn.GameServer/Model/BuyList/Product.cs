using NLog;

namespace L2Dn.GameServer.Model.BuyList;

public class Product
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Product));
	
	private readonly int _buyListId;
	private readonly ItemTemplate _item;
	private readonly long _price;
	private readonly long _restockDelay;
	private readonly long _maxCount;
	private readonly double _baseTax;
	private long _count;
	
	public Product(int buyListId, ItemTemplate item, long price, long restockDelay, long maxCount, int baseTax)
	{
		Objects.requireNonNull(item);
		_buyListId = buyListId;
		_item = item;
		_price = (price < 0) ? item.getReferencePrice() : price;
		_restockDelay = restockDelay * 60000;
		_maxCount = maxCount;
		_baseTax = baseTax / 100.0;
		if (hasLimitedStock())
		{
			_count = maxCount;
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
		if (_item.getItemType().equals(EtcItemType.CASTLE_GUARD))
		{
			price *= Config.RATE_SIEGE_GUARDS_PRICE;
		}
		return price;
	}
	
	public double getBaseTaxRate()
	{
		return _baseTax;
	}
	
	public long getRestockDelay()
	{
		return _restockDelay;
	}
	
	public long getMaxCount()
	{
		return _maxCount;
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
		if (_count == null)
		{
			return false;
		}
		
		BuyListTaskManager.getInstance().add(this, System.currentTimeMillis() + _restockDelay);
		
		bool result = Interlocked.Add(ref _count, -value) >= 0;
		save();
		return result;
	}
	
	public bool hasLimitedStock()
	{
		return _maxCount > -1;
	}
	
	public void restartRestockTask(long nextRestockTime)
	{
		long remainTime = nextRestockTime - System.currentTimeMillis();
		if (remainTime > 0)
		{
			BuyListTaskManager.getInstance().update(this, remainTime);
		}
		else
		{
			restock();
		}
	}
	
	public void restock()
	{
		setCount(_maxCount);
		save();
	}
	
	private void save()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(
				"INSERT INTO `buylists`(`buylist_id`, `item_id`, `count`, `next_restock_time`) VALUES(?, ?, ?, ?) ON DUPLICATE KEY UPDATE `count` = ?, `next_restock_time` = ?");
			statement.setInt(1, _buyListId);
			statement.setInt(2, _item.getId());
			statement.setLong(3, getCount());
			statement.setLong(5, getCount());
			
			long nextRestockTime = BuyListTaskManager.getInstance().getRestockDelay(this);
			if (nextRestockTime > 0)
			{
				statement.setLong(4, nextRestockTime);
				statement.setLong(6, nextRestockTime);
			}
			else
			{
				statement.setLong(4, 0);
				statement.setLong(6, 0);
			}
			
			statement.executeUpdate();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Failed to save Product buylist_id:" + _buyListId + " item_id=" + _item.getId() + ": " + e);
		}
	}
}
