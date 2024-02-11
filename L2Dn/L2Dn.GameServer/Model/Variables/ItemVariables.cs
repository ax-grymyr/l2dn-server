using NLog;

namespace L2Dn.GameServer.Model.Variables;

public class ItemVariables: AbstractVariables
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemVariables));
	
	// SQL Queries.
	private const String SELECT_QUERY = "SELECT * FROM item_variables WHERE id = ?";
	private const String SELECT_COUNT = "SELECT COUNT(*) FROM item_variables WHERE id = ?";
	private const String DELETE_QUERY = "DELETE FROM item_variables WHERE id = ?";
	private const String INSERT_QUERY = "INSERT INTO item_variables (id, var, val) VALUES (?, ?, ?)";
	
	private readonly int _objectId;
	
	// Static Constants
	public const String VISUAL_ID = "visualId";
	public const String VISUAL_APPEARANCE_STONE_ID = "visualAppearanceStoneId";
	public const String VISUAL_APPEARANCE_LIFE_TIME = "visualAppearanceLifetime";
	public const String BLESSED = "blessed";
	
	public ItemVariables(int objectId)
	{
		_objectId = objectId;
		restoreMe();
	}
	
	public static bool hasVariables(int objectId)
	{
		// Restore previous variables.
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement st = con.prepareStatement(SELECT_COUNT);
			st.setInt(1, objectId);
			ResultSet rset = st.executeQuery();
			if (rset.next())
			{
				return rset.getInt(1) > 0;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(nameof(ItemVariables) + ": Couldn't select variables count for: " + objectId + ": " + e);
			return false;
		}
		return true;
	}
	
	public override bool restoreMe()
	{
		// Restore previous variables.
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement st = con.prepareStatement(SELECT_QUERY);
			st.setInt(1, _objectId);
			ResultSet rset = st.executeQuery();
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
			PreparedStatement st1 = con.prepareStatement(DELETE_QUERY);
			st1.setInt(1, _objectId);
			st1.execute();
			
			// Insert all variables.
			PreparedStatement st2 = con.prepareStatement(INSERT_QUERY);
			st2.setInt(1, _objectId);
			foreach (Entry<String, Object> entry in getSet().entrySet())
			{
				st2.setString(2, entry.getKey());
				st2.setString(3, String.valueOf(entry.getValue()));
				st2.addBatch();
			}
			st2.executeBatch();
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
}