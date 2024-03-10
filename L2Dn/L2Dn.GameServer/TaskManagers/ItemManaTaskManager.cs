using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Mobius
 */
public class ItemManaTaskManager: Runnable
{
	private static readonly Map<Item, DateTime> ITEMS = new();
	private const int MANA_CONSUMPTION_RATE = 60000;
	private static bool _working = false;
	
	protected ItemManaTaskManager()
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
		
		if (!ITEMS.isEmpty())
		{
			DateTime currentTime = DateTime.UtcNow;
			List<Item> toRemove = new List<Item>();
			foreach (var entry in ITEMS)
			{
				if (currentTime > entry.Value)
				{
					Item item = entry.Key;
					toRemove.Add(item);
					
					Player player = item.getActingPlayer();
					if ((player == null) || player.isInOfflineMode())
					{
						continue;
					}
					
					item.decreaseMana(item.isEquipped());
				}
			}

			foreach (Item item in toRemove)
			{
				ITEMS.remove(item);
			}
		}
		
		_working = false;
	}
	
	public void add(Item item)
	{
		if (!ITEMS.containsKey(item))
		{
			ITEMS.put(item, DateTime.UtcNow.AddMilliseconds(MANA_CONSUMPTION_RATE));
		}
	}
	
	public static ItemManaTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemManaTaskManager INSTANCE = new ItemManaTaskManager();
	}
}