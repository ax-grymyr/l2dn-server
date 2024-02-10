using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Sql;

public class CharInfoTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CharInfoTable));
	
	private readonly Map<int, String> _names = new();
	private readonly Map<int, int> _accessLevels = new();
	private readonly Map<int, int> _levels = new();
	private readonly Map<int, int> _classes = new();
	private readonly Map<int, int> _clans = new();
	private readonly Map<int, Map<int, String>> _memos = new();
	private readonly Map<int, Calendar> _creationDates = new();
	private readonly Map<int, long> _lastAccess = new();
	
	protected CharInfoTable()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement s = con.createStatement();
			ResultSet rs = s.executeQuery("SELECT charId, char_name, accesslevel FROM characters");
			while (rs.next())
			{
				int id = rs.getInt("charId");
				_names.put(id, rs.getString("char_name"));
				_accessLevels.put(id, rs.getInt("accesslevel"));
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't retrieve all char id/name/access: " + e);
		}
		LOGGER.Info(GetType().Name + ": Loaded " + _names.size() + " char names.");
	}
	
	public void addName(Player player)
	{
		if (player != null)
		{
			addName(player.getObjectId(), player.getName());
			_accessLevels.put(player.getObjectId(), player.getAccessLevel().getLevel());
		}
	}
	
	private void addName(int objectId, String name)
	{
		if ((name != null) && !name.equals(_names.get(objectId)))
		{
			_names.put(objectId, name);
		}
	}
	
	public void removeName(int objId)
	{
		_names.remove(objId);
		_accessLevels.remove(objId);
	}
	
	public int getIdByName(String name)
	{
		if ((name == null) || name.isEmpty())
		{
			return -1;
		}
		
		for (Entry<int, String> entry : _names.entrySet())
		{
			if (entry.getValue().equalsIgnoreCase(name))
			{
				return entry.getKey();
			}
		}
		
		// Should not continue after the above?
		
		int id = -1;
		int accessLevel = 0;
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT charId,accesslevel FROM characters WHERE char_name=?");
			ps.setString(1, name);
			{
				ResultSet rs = ps.executeQuery();
				while (rs.next())
				{
					id = rs.getInt("charId");
					accessLevel = rs.getInt("accesslevel");
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing char name: " + e);
		}
		
		if (id > 0)
		{
			_names.put(id, name);
			_accessLevels.put(id, accessLevel);
			return id;
		}
		
		return -1; // Not found.
	}
	
	public String getNameById(int id)
	{
		if (id <= 0)
		{
			return null;
		}
		
		String name = _names.get(id);
		if (name != null)
		{
			return name;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT char_name,accesslevel FROM characters WHERE charId=?");
			ps.setInt(1, id);

			{
				ResultSet rset = ps.executeQuery();
				if (rset.next())
				{
					name = rset.getString("char_name");
					_names.put(id, name);
					_accessLevels.put(id, rset.getInt("accesslevel"));
					return name;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing char id: " + e);
		}
		
		return null; // not found
	}
	
	public int getAccessLevelById(int objectId)
	{
		return getNameById(objectId) != null ? _accessLevels.get(objectId) : 0;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public bool doesCharNameExist(String name)
	{
		bool result = false;
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT COUNT(*) as count FROM characters WHERE char_name=?");
			ps.setString(1, name);

			{
				ResultSet rs = ps.executeQuery();
				if (rs.next())
				{
					result = rs.getInt("count") > 0;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing charname: " + e);
		}
		return result;
	}
	
	public int getAccountCharacterCount(String account)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps =
				con.prepareStatement("SELECT COUNT(char_name) as count FROM characters WHERE account_name=?");
			ps.setString(1, account);

			{
				ResultSet rset = ps.executeQuery();
				if (rset.next())
				{
					return rset.getInt("count");
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Couldn't retrieve account for id: " + e);
		}
		return 0;
	}
	
	public void setLevel(int objectId, int level)
	{
		_levels.put(objectId, level);
	}
	
	public int getLevelById(int objectId)
	{
		int level = _levels.get(objectId);
		if (level != null)
		{
			return level;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT level FROM characters WHERE charId = ?");
			ps.setInt(1, objectId);

			{
				ResultSet rset = ps.executeQuery();
				if (rset.next())
				{
					int dbLevel = rset.getInt("level");
					_levels.put(objectId, dbLevel);
					return dbLevel;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing char count: " + e);
		}
		return 0;
	}
	
	public void setClassId(int objectId, int classId)
	{
		_classes.put(objectId, classId);
	}
	
	public int getClassIdById(int objectId)
	{
		int classId = _classes.get(objectId);
		if (classId != null)
		{
			return classId;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT classid FROM characters WHERE charId = ?");
			ps.setInt(1, objectId);

			{
				ResultSet rset = ps.executeQuery();
				if (rset.next())
				{
					int dbClassId = rset.getInt("classid");
					_classes.put(objectId, dbClassId);
					return dbClassId;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't retrieve class for id: " + e);
		}
		return 0;
	}
	
	public void setClanId(int objectId, int clanId)
	{
		_clans.put(objectId, clanId);
	}
	
	public void removeClanId(int objectId)
	{
		_clans.remove(objectId);
	}
	
	public int getClanIdById(int objectId)
	{
		int clanId = _clans.get(objectId);
		if (clanId != null)
		{
			return clanId;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT clanId FROM characters WHERE charId = ?");
			ps.setInt(1, objectId);

			{
				ResultSet rset = ps.executeQuery();
				while (rset.next())
				{
					int dbClanId = rset.getInt("clanId");
					_clans.put(objectId, dbClanId);
					return dbClanId;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing char count: " + e);
		}
		
		// Prevent searching again.
		_clans.put(objectId, 0);
		return 0;
	}
	
	public void setFriendMemo(int charId, int friendId, String memo)
	{
		Map<int, String> memos = _memos.get(charId);
		if (memos == null)
		{
			memos = new();
			_memos.put(charId, memos);
		}
		
		if (memo == null)
		{
			memos.put(friendId, "");
			return;
		}
		
		// Bypass exploit check.
		String text = memo.ToLower();
		if (text.Contains("action") && text.Contains("bypass"))
		{
			memos.put(friendId, "");
			return;
		}
		
		// Add text without tags.
		memos.put(friendId, memo.replaceAll("<.*?>", ""));
	}
	
	public void removeFriendMemo(int charId, int friendId)
	{
		Map<int, String> memos = _memos.get(charId);
		if (memos == null)
		{
			return;
		}
		
		memos.remove(friendId);
	}
	
	public String getFriendMemo(int charId, int friendId)
	{
		Map<int, String> memos = _memos.get(charId);
		if (memos == null)
		{
			memos = new();
			_memos.put(charId, memos);
		}
		else if (memos.containsKey(friendId))
		{
			return memos.get(friendId);
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement =
				con.prepareStatement("SELECT memo FROM character_friends WHERE charId=? AND friendId=?");
			statement.setInt(1, charId);
			statement.setInt(2, friendId);


			{
				ResultSet rset = statement.executeQuery();
				if (rset.next())
				{
					String dbMemo = rset.getString("memo");
					memos.put(friendId, dbMemo == null ? "" : dbMemo);
					return dbMemo;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error occurred while retrieving memo: " + e);
		}
		
		// Prevent searching again.
		memos.put(friendId, "");
		return null;
	}
	
	public Calendar getCharacterCreationDate(int objectId)
	{
		Calendar calendar = _creationDates.get(objectId);
		if (calendar != null)
		{
			return calendar;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT createDate FROM characters WHERE charId = ?");
			ps.setInt(1, objectId);

			{
				ResultSet rset = ps.executeQuery();
				if (rset.next())
				{
					Date createDate = rset.getDate("createDate");
					Calendar newCalendar = Calendar.getInstance();
					newCalendar.setTime(createDate);
					_creationDates.put(objectId, newCalendar);
					return newCalendar;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not retrieve character creation date: " + e);
		}
		return null;
	}
	
	public void setLastAccess(int objectId, long lastAccess)
	{
		_lastAccess.put(objectId, lastAccess);
	}
	
	public int getLastAccessDelay(int objectId)
	{
		long lastAccess = _lastAccess.get(objectId);
		if (lastAccess != null)
		{
			long currentTime = System.currentTimeMillis();
			long timeDifferenceInMillis = currentTime - lastAccess;
			return (int) (timeDifferenceInMillis / 1000);
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT lastAccess FROM characters WHERE charId = ?");
			ps.setInt(1, objectId);

			{
				ResultSet rset = ps.executeQuery();
				if (rset.next())
				{
					long dbLastAccess = rset.getLong("lastAccess");
					_lastAccess.put(objectId, dbLastAccess);
					
					long currentTime = System.currentTimeMillis();
					long timeDifferenceInMillis = currentTime - dbLastAccess;
					return (int) (timeDifferenceInMillis / 1000);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not retrieve lastAccess timestamp: " + e);
		}
		return 0;
	}
	
	public static CharInfoTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CharInfoTable INSTANCE = new CharInfoTable();
	}
}