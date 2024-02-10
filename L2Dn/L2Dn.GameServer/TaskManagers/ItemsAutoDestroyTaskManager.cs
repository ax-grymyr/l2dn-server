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
		
		DateTime currentTime = DateTime.Now;
		Iterator<Item> iterator = ITEMS.iterator();
		Item itemInstance;
		
		while (iterator.hasNext())
		{
			itemInstance = iterator.next();
			if ((itemInstance.getDropTime() == 0) || (itemInstance.getItemLocation() != ItemLocation.VOID))
			{
				iterator.remove();
			}
			else
			{
				long autoDestroyTime;
				if (itemInstance.getTemplate().getAutoDestroyTime() > 0)
				{
					autoDestroyTime = itemInstance.getTemplate().getAutoDestroyTime();
				}
				else if (itemInstance.getTemplate().hasExImmediateEffect())
				{
					autoDestroyTime = Config.HERB_AUTO_DESTROY_TIME;
				}
				else
				{
					autoDestroyTime = ((Config.AUTODESTROY_ITEM_AFTER == 0) ? 3600000 : Config.AUTODESTROY_ITEM_AFTER * 1000);
				}
				
				if ((currentTime - itemInstance.getDropTime()) > autoDestroyTime)
				{
					itemInstance.decayMe();
					if (Config.SAVE_DROPPED_ITEM)
					{
						ItemsOnGroundManager.getInstance().removeObject(itemInstance);
					}
					iterator.remove();
				}
			}
		}
	}
	
	public void addItem(Item item)
	{
		item.setDropTime(DateTime.Now);
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