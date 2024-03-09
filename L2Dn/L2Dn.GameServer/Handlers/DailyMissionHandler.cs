using L2Dn.GameServer.Handlers.DailyMissionHandlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Scripting;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Sdw
 */
public class DailyMissionHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(DailyMissionHandler));
	private readonly Map<String, Func<DailyMissionDataHolder, AbstractDailyMissionHandler>> _handlerFactories = new();

	private DailyMissionHandler()
	{
		registerHandler("level", h => new LevelDailyMissionHandler(h));
		registerHandler("loginweekend", h => new LoginWeekendDailyMissionHandler(h));
		registerHandler("loginmonth", h => new LoginMonthDailyMissionHandler(h));
		registerHandler("quest", h => new QuestDailyMissionHandler(h));
		registerHandler("olympiad", h => new OlympiadDailyMissionHandler(h));
		registerHandler("siege", h => new SiegeDailyMissionHandler(h));
		registerHandler("boss", h => new BossDailyMissionHandler(h));
		registerHandler("monster", h => new MonsterDailyMissionHandler(h));
		registerHandler("fishing", h => new FishingDailyMissionHandler(h));
		registerHandler("spirit", h => new SpiritDailyMissionHandler(h));
		registerHandler("joinclan", h => new JoinClanDailyMissionHandler(h));
		registerHandler("purge", h => new PurgeRewardDailyMissionHandler(h));
		registerHandler("useitem", h => new UseItemDailyMissionHandler(h));
		_logger.Info(GetType().Name + ": Loaded " + size() + " handlers.");
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
	
	public void executeScript()
	{
		try
		{
			ScriptEngineManager.getInstance().executeScript(ScriptEngineManager.ONE_DAY_REWARD_MASTER_HANDLER);
		}
		catch (Exception e)
		{
			throw new Exception("Problems while running DailyMissionMasterHandler", e);
		}
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