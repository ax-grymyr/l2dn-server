using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

public class ItemsAutoDestroyTaskManager: Runnable
{
	private static readonly Set<Item> ITEMS = new ();
	
	protected ItemsAutoDestroyTaskManager()
	{
		ThreadPool.scheduleAtFixedRate(this, 5000, 5000);
	}
	
	public void run()
	{
		if (ITEMS.isEmpty())
		{
			return;
		}
		
		DateTime currentTime = DateTime.UtcNow;
		List<Item> toRemove = new List<Item>();
		foreach (Item itemInstance in ITEMS)
		{
			if ((itemInstance.getDropTime() is null) || (itemInstance.getItemLocation() != ItemLocation.VOID))
			{
				toRemove.Add(itemInstance);
			}
			else
			{
				TimeSpan autoDestroyTime;
				if (itemInstance.getTemplate().getAutoDestroyTime() > TimeSpan.Zero)
				{
					autoDestroyTime = itemInstance.getTemplate().getAutoDestroyTime().Value;
				}
				else if (itemInstance.getTemplate().hasExImmediateEffect())
				{
					autoDestroyTime = TimeSpan.FromMilliseconds(Config.HERB_AUTO_DESTROY_TIME);
				}
				else
				{
					autoDestroyTime = ((Config.AUTODESTROY_ITEM_AFTER == 0)
						? TimeSpan.FromMilliseconds(3600000)
						: TimeSpan.FromMilliseconds(Config.AUTODESTROY_ITEM_AFTER * 1000));
				}
				
				if ((currentTime - itemInstance.getDropTime()) > autoDestroyTime)
				{
					itemInstance.decayMe();
					if (Config.SAVE_DROPPED_ITEM)
					{
						ItemsOnGroundManager.getInstance().removeObject(itemInstance);
					}

					toRemove.Add(itemInstance);
				}
			}
		}

		foreach (Item item in toRemove)
		{
			ITEMS.remove(item);
		}
	}
	
	public void addItem(Item item)
	{
		item.setDropTime(DateTime.UtcNow);
		ITEMS.add(item);
	}
	
	public static ItemsAutoDestroyTaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemsAutoDestroyTaskManager INSTANCE = new ItemsAutoDestroyTaskManager();
	}
}