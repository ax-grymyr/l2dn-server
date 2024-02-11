using NLog;

namespace L2Dn.GameServer.Model.Variables;

public class ClanVariables: AbstractVariables
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanVariables));
	
	// SQL Queries.
	private const String SELECT_QUERY = "SELECT * FROM clan_variables WHERE clanId = ?";
	private const String DELETE_QUERY = "DELETE FROM clan_variables WHERE clanId = ?";
	private const String INSERT_QUERY = "INSERT INTO clan_variables (clanId, var, val) VALUES (?, ?, ?)";
	private const String DELETE_WEAKLY_QUERY = "DELETE FROM clan_variables WHERE var LIKE 'CONTRIBUTION_WEEKLY_%' AND clanId = ?";
	
	// Public variable names.
	public const String CONTRIBUTION = "CONTRIBUTION_";
	public const String CONTRIBUTION_WEEKLY = "CONTRIBUTION_WEEKLY_";
	
	private readonly int _objectId;
	
	public ClanVariables(int objectId)
	{
		_objectId = objectId;
		restoreMe();
	}
	
	public override bool restoreMe()
	{
		// Restore previous variables.
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement st = con.prepareStatement(SELECT_QUERY);
			st.setInt(1, _objectId);
			ResultSet rset = st.executeQuery()
			while (rset.next())
			{
				set(rset.getString("var"), rset.getString("val"), false);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't restore variables for: " + _objectId + ": " + e);
			return false;
		}
		finally
		{
			compareAndSetChanges(true, false);
		}
		return true;
	}
	
	public override bool storeMe()
	{
		// No changes, nothing to store.
		if (!hasChanges())
		{
			return false;
		}
		
		try
		{
			using GameServerDbContext ctx = new();
			// Clear previous entries.
			PreparedStatement st = con.prepareStatement(DELETE_QUERY);
			st.setInt(1, _objectId);
			st.execute();
			
			// Insert all variables.
			PreparedStatement st = con.prepareStatement(INSERT_QUERY);
			st.setInt(1, _objectId);
			foreach (Entry<String, Object> entry in getSet().entrySet())
			{
				st.setString(2, entry.getKey());
				st.setString(3, String.valueOf(entry.getValue()));
				st.addBatch();
			}
			st.executeBatch();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't update variables for: " + _objectId + ": " + e);
			return false;
		}
		finally
		{
			compareAndSetChanges(true, false);
		}
		return true;
	}
	
	public override bool deleteMe()
	{
		try
		{
			using GameServerDbContext ctx = new();
			// Clear previous entries.
			PreparedStatement st = con.prepareStatement(DELETE_QUERY);
			st.setInt(1, _objectId);
			st.execute();
			
			// Clear all entries
			getSet().clear();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't delete variables for: " + _objectId + ": " + e);
			return false;
		}
		return true;
	}

	public bool deleteWeeklyContribution()
	{
		try
		{
			using GameServerDbContext ctx = new();
			// Clear previous entries.
			PreparedStatement st = con.prepareStatement(DELETE_WEAKLY_QUERY);
			st.setInt(1, _objectId);
			st.execute();

			// Clear all entries
			getSet().entrySet().stream().filter(it => it.getKey().startsWith("CONTRIBUTION_WEEKLY_"))
				.collect(Collectors.toList()).forEach(it => getSet().remove(it.getKey()));
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't delete variables for: " + _objectId + ": " + e);
			return false;
		}

		return true;
	}
}
