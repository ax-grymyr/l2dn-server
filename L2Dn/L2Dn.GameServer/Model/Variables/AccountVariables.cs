using NLog;

namespace L2Dn.GameServer.Model.Variables;

public class AccountVariables: AbstractVariables
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AccountVariables));
	
	// SQL Queries.
	private const String SELECT_QUERY = "SELECT * FROM account_gsdata WHERE account_name = ?";
	private const String DELETE_QUERY = "DELETE FROM account_gsdata WHERE account_name = ?";
	private const String INSERT_QUERY = "REPLACE INTO account_gsdata (account_name, var, value) VALUES (?, ?, ?)";
	private const String DELETE_QUERY_VAR = "DELETE FROM account_gsdata where var = ?";
	
	// Public variable names
	public const String HWID = "HWID";
	public const String HWIDSLIT_VAR = "	";
	public const String PRIME_SHOP_PRODUCT_COUNT = "PSPCount";
	public const String PRIME_SHOP_PRODUCT_DAILY_COUNT = "PSPDailyCount";
	public const String LCOIN_SHOP_PRODUCT_COUNT = "LCSCount";
	public const String LCOIN_SHOP_PRODUCT_DAILY_COUNT = "LCSDailyCount";
	public const String LCOIN_SHOP_PRODUCT_MONTLY_COUNT = "LCSMontlyCount";
	public const String VIP_POINTS = "VipPoints";
	public const String VIP_TIER = "VipTier";
	public const String VIP_EXPIRATION = "VipExpiration";
	public const String VIP_ITEM_BOUGHT = "Vip_Item_Bought";
	
	private readonly String _accountName;
	
	public AccountVariables(String accountName)
	{
		_accountName = accountName;
		restoreMe();
	}
	
	public override bool restoreMe()
	{
		// Restore previous variables.
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement st = con.prepareStatement(SELECT_QUERY);
			st.setString(1, _accountName);
			ResultSet rset = st.executeQuery();
			while (rset.next())
			{
				set(rset.getString("var"), rset.getString("value"));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't restore variables for: " + _accountName + ": " + e);
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
			st.setString(1, _accountName);
			st.execute();
			
			// Insert all variables.
			PreparedStatement st = con.prepareStatement(INSERT_QUERY);
			st.setString(1, _accountName);
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
			LOGGER.Warn(GetType().Name + ": Couldn't update variables for: " + _accountName + ": " + e);
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
			st.setString(1, _accountName);
			st.execute();
			
			// Clear all entries
			getSet().clear();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't delete variables for: " + _accountName + ": " + e);
			return false;
		}
		return true;
	}
	
	/**
	 * Delete all entries for an requested var
	 * @param var
	 * @return success
	 */
	public static bool deleteVipPurchases(String var)
	{
		try
		{
			using GameServerDbContext ctx = new();
			// Clear previous entries.
			PreparedStatement st = con.prepareStatement(DELETE_QUERY_VAR);
			st.setString(1, var);
			st.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn("AccountVariables: Couldn't delete vip variables: " + e);
			return false;
		}
		return true;
	}
}