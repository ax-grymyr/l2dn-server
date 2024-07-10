using L2Dn.GameServer.Model.BuyList;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class BuyListTaskManager
{
	protected static readonly Map<Product, DateTime> PRODUCTS = new();
	protected static readonly List<Product> PENDING_UPDATES = new();
	protected static bool _workingProducts = false;
	protected static bool _workingSaves = false;
	
	protected BuyListTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(new BuyListProductTask(), 1000, 60000);
		ThreadPool.scheduleAtFixedRate(new BuyListSaveTask(), 50, 50);
	}
	
	protected class BuyListProductTask: Runnable
	{
		public void run()
		{
			if (_workingProducts)
			{
				return;
			}
			_workingProducts = true;
			
			DateTime currentTime = DateTime.Now;
			foreach (var entry in PRODUCTS)
			{
				if (currentTime > entry.Value)
				{
					Product product = entry.Key;
					PRODUCTS.remove(product);
					lock (PENDING_UPDATES)
					{
						if (!PENDING_UPDATES.Contains(product))
						{
							PENDING_UPDATES.add(product);
						}
					}
				}
			}
			
			_workingProducts = false;
		}
	}
	
	protected class BuyListSaveTask: Runnable
	{
		public void run()
		{
			if (_workingSaves)
			{
				return;
			}
			_workingSaves = true;
			
			if (!PENDING_UPDATES.isEmpty())
			{
				Product product;
				lock (PENDING_UPDATES)
				{
					product = PENDING_UPDATES[0];
					PENDING_UPDATES.Remove(product);
				}
				product.restock();
			}
			
			_workingSaves = false;
		}
	}
	
	public void add(Product product, DateTime endTime)
	{
		if (!PRODUCTS.containsKey(product))
		{
			PRODUCTS.put(product, endTime);
		}
	}
	
	public void update(Product product, DateTime endTime)
	{
		PRODUCTS.put(product, endTime);
	}
	
	public DateTime? getRestockDelay(Product product)
	{
		return PRODUCTS.TryGetValue(product, out DateTime time) ? time : null;
	}
	
	public static BuyListTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly BuyListTaskManager INSTANCE = new BuyListTaskManager();
	}
}