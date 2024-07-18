using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class ItemAppearanceTaskManager: Runnable
{
	private static readonly Map<Item, DateTime> ITEMS = new();
	private static bool _working = false;
	
	protected ItemAppearanceTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(this, 1000, 1000);
	}
	
	public void run()
	{
		if (_working)
		{
			return;
		}
		_working = true;
		
		if (ITEMS.Count != 0)
		{
			DateTime currentTime = DateTime.UtcNow;
			List<Item> itemsToDelete = ITEMS.Where(x => currentTime > x.Value).Select(x => x.Key).ToList();
			foreach (Item item in itemsToDelete)
			{
				item.onVisualLifeTimeEnd();
				ITEMS.remove(item);
			}
		}
		
		_working = false;
	}
	
	public void add(Item item, DateTime endTime)
	{
		if (!ITEMS.ContainsKey(item))
		{
			ITEMS.put(item, endTime);
		}
	}
	
	public void remove(Item item)
	{
		ITEMS.remove(item);
	}
	
	public static ItemAppearanceTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemAppearanceTaskManager INSTANCE = new ItemAppearanceTaskManager();
	}
}