using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Sdw
 */
public class DailyMissionHandler
{
	private readonly Map<string, Func<DailyMissionDataHolder, AbstractDailyMissionHandler>> _handlerFactories = new();

	private DailyMissionHandler()
	{
		//_logger.Info(GetType().Name + ": Loaded " + size() + " handlers.");
	}

	public void registerHandler(string name, Func<DailyMissionDataHolder, AbstractDailyMissionHandler> handlerFactory)
	{
		_handlerFactories.put(name, handlerFactory);
	}

	public Func<DailyMissionDataHolder, AbstractDailyMissionHandler>? getHandler(string name)
	{
		return _handlerFactories.GetValueOrDefault(name);
	}

	public static DailyMissionHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly DailyMissionHandler INSTANCE = new();
	}
}