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
			DateTime currentTime = DateTime.Now;
			Iterator<Entry<Item, long>> iterator = ITEMS.entrySet().iterator();
			Entry<Item, long> entry;
			
			while (iterator.hasNext())
			{
				entry = iterator.next();
				if (currentTime > entry.getValue())
				{
					iterator.remove();
					
					Item item = entry.getKey();
					Player player = item.getActingPlayer();
					if ((player == null) || player.isInOfflineMode())
					{
						continue;
					}
					
					item.decreaseMana(item.isEquipped());
				}
			}
		}
		
		_working = false;
	}
	
	public void add(Item item)
	{
		if (!ITEMS.containsKey(item))
		{
			ITEMS.put(item, DateTime.Now.AddMilliseconds(MANA_CONSUMPTION_RATE));
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