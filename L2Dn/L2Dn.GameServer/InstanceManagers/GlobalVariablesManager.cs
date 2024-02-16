using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Variables;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Global Variables Manager.
 * @author xban1x
 */
public class GlobalVariablesManager: AbstractVariables
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(GlobalVariablesManager));
	
	// Public variable names
	public static readonly String DAILY_TASK_RESET = "DAILY_TASK_RESET";
	public static readonly String MONSTER_ARENA_VARIABLE = "MA_C";
	public static readonly String RANKING_POWER_COOLDOWN = "RANKING_POWER_COOLDOWN";
	public static readonly String RANKING_POWER_LOCATION = "RANKING_POWER_LOCATION";
	public static readonly String PURGE_REWARD_TIME = "PURGE_REWARD_TIME";
	public static readonly String BALOK_REMAIN_TIME = "BALOK_REMAIN_TIME";
	
	protected GlobalVariablesManager()
	{
		restoreMe();
	}
	
	public bool restoreMe()
	{
		// Restore previous variables.
		try 
		{
			using GameServerDbContext ctx = new();
			foreach (GlobalVariable record in ctx.GlobalVariables)
			{
				set(record.Name, record.Value);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Couldn't restore global variables.");
			return false;
		}
		
		LOGGER.Info(GetType().Name +": Loaded " + getSet().size() + " variables.");
		return true;
	}
	
	public bool storeMe()
	{
		try 
		{
			using GameServerDbContext ctx = new();

			// Clear previous entries.
			ctx.GlobalVariables.ExecuteDelete();

			ctx.GlobalVariables.AddRange(getSet().Select(pair => new GlobalVariable()
			{
				Name = pair.Key,
				Value = pair.Value?.ToString()
			}));

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't save global variables to database." + e);
			return false;
		}
		
		LOGGER.Info(GetType().Name +": Stored " + getSet().size() + " variables.");
		return true;
	}
	
	public bool deleteMe()
	{
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.GlobalVariables.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't delete global variables to database." + e);
			return false;
		}
		return true;
	}
	
	/**
	 * Gets the single instance of {@code GlobalVariablesManager}.
	 * @return single instance of {@code GlobalVariablesManager}
	 */
	public static GlobalVariablesManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly GlobalVariablesManager INSTANCE = new GlobalVariablesManager();
	}
}