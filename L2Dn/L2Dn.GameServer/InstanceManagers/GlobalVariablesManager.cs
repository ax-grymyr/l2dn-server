using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Variables;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Global Variables Manager.
 * @author xban1x
 */
public class GlobalVariablesManager: AbstractVariables<GlobalVariable>
{
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

	protected override IQueryable<GlobalVariable> GetQuery(GameServerDbContext ctx)
	{
		return ctx.GlobalVariables;
	}

	protected override GlobalVariable CreateVar()
	{
		return new GlobalVariable();
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