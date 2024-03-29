using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers.Events;

/**
 * @author Mobius
 */
public class ItemDeletionInfoManager
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemDeletionInfoManager));
	
	private readonly Map<int, DateTime> _itemDates = new();
	
	protected ItemDeletionInfoManager()
	{
	}
	
	public void addItemDate(int itemId, DateTime date)
	{
		_itemDates.put(itemId, date);
	}
	
	public Map<int, DateTime> getItemDates()
	{
		return _itemDates;
	}
	
	public static ItemDeletionInfoManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemDeletionInfoManager INSTANCE = new ItemDeletionInfoManager();
	}
}