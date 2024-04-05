using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Sdw
 */
public class DailyMissionHandler
{
	private readonly Map<String, Func<DailyMissionDataHolder, AbstractDailyMissionHandler>> _handlerFactories = new();

	private DailyMissionHandler()
	{
		//_logger.Info(GetType().Name + ": Loaded " + size() + " handlers.");
	}
	
	public void registerHandler(String name, Func<DailyMissionDataHolder, AbstractDailyMissionHandler> handlerFactory)
	{
		_handlerFactories.put(name, handlerFactory);
	}
	
	public Func<DailyMissionDataHolder, AbstractDailyMissionHandler> getHandler(String name)
	{
		return _handlerFactories.get(name);
	}
	
	public int size()
	{
		return _handlerFactories.size();
	}
	
	public static DailyMissionHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly DailyMissionHandler INSTANCE = new DailyMissionHandler();
	}
}