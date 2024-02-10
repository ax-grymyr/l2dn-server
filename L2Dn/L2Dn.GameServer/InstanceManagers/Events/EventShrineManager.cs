namespace L2Dn.GameServer.InstanceManagers.Events;

/**
 * @author Mobius
 */
public class EventShrineManager
{
	private static bool ENABLE_SHRINES = false;
	
	public bool areShrinesEnabled()
	{
		return ENABLE_SHRINES;
	}
	
	public void setEnabled(bool enabled)
	{
		ENABLE_SHRINES = enabled;
	}
	
	public static EventShrineManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly EventShrineManager INSTANCE = new EventShrineManager();
	}
}