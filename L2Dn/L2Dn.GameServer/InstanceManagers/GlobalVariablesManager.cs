using L2Dn.GameServer.Model.Variables;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Global Variables Manager.
 * @author xban1x
 */
public class GlobalVariablesManager: AbstractVariables
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(GlobalVariablesManager));
	
	// SQL Queries.
	private const string SELECT_QUERY = "SELECT * FROM global_variables";
	private const string DELETE_QUERY = "DELETE FROM global_variables";
	private const string INSERT_QUERY = "INSERT INTO global_variables (var, value) VALUES (?, ?)";
	
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
			Statement st = con.createStatement();
			ResultSet rset = st.executeQuery(SELECT_QUERY);
			while (rset.next())
			{
				set(rset.getString("var"), rset.getString("value"));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't restore global variables.");
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
			Statement del = con.createStatement();
			PreparedStatement st = con.prepareStatement(INSERT_QUERY);
			// Clear previous entries.
			del.execute(DELETE_QUERY);
			
			// Insert all variables.
			foreach (var entry in getSet())
			{
				st.setString(1, entry.Key);
				st.setString(2, entry.Value.ToString());
				st.addBatch();
			}
			st.executeBatch();
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
			Statement del = con.createStatement();
			del.execute(DELETE_QUERY);
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