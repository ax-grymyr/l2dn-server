using System.Runtime.CompilerServices;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using NLog;

namespace L2Dn.GameServer.Data.Sql;

public class CharInfoTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CharInfoTable));
	
	private readonly Map<int, string> _names = new();
	private readonly Map<int, int> _accessLevels = new();
	private readonly Map<int, int> _levels = new();
	private readonly Map<int, CharacterClass> _classes = new();
	private readonly Map<int, int> _clans = new();
	private readonly Map<int, Map<int, string>> _memos = new();
	private readonly Map<int, DateTime> _creationDates = new();
	private readonly Map<int, DateTime> _lastAccess = new();
	
	protected CharInfoTable()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var chars = ctx.Characters.Select(c => new { c.Id, c.Name, c.AccessLevel });

			foreach (var ch in chars)
			{
				int id = ch.Id;
				_names.put(id, ch.Name);
				_accessLevels.put(id, ch.AccessLevel);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Couldn't retrieve all char id/name/access: " + e);
		}

		LOGGER.Info(GetType().Name + ": Loaded " + _names.Count + " char names.");
	}
	
	public void addName(Player player)
	{
		if (player != null)
		{
			addName(player.ObjectId, player.getName());
			_accessLevels.put(player.ObjectId, player.getAccessLevel().getLevel());
		}
	}
	
	private void addName(int objectId, string name)
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
	
	public int getIdByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return -1;
		}
		
		foreach (var entry in _names)
		{
			if (entry.Value.equalsIgnoreCase(name))
			{
				return entry.Key;
			}
		}
		
		// Should not continue after the above?
		
		int id = -1;
		int accessLevel = 0;
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var ch = ctx.Characters.Where(c => c.Name == name).Select(c => new { c.Id, c.AccessLevel })
				.SingleOrDefault();

			if (ch is not null)
			{
				id = ch.Id;
				accessLevel = ch.AccessLevel;
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
	
	public string? getNameById(int id)
	{
		if (id <= 0)
		{
			return null;
		}
		
		string? name = _names.get(id);
		if (name != null)
		{
			return name;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var ch = ctx.Characters.Where(c => c.Id == id).Select(c => new { c.Name, c.AccessLevel })
				.SingleOrDefault();

			if (ch is not null)
			{
				name = ch.Name;
				_names.put(id, name);
				_accessLevels.put(id, ch.AccessLevel);
				return name;
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
	public bool doesCharNameExist(string name)
	{
		bool result = false;
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			result = ctx.Characters.Any(c => c.Name == name);
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing charname: " + e);
		}
		return result;
	}
	
	public int getAccountCharacterCount(string account)
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			return ctx.Characters.Count(c => c.Name == account);
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
		if (_levels.TryGetValue(objectId, out int level))
		{
			return level;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			byte? lv = ctx.Characters.Where(c => c.Id == objectId).Select(c => (byte?)c.Level).SingleOrDefault();
			if (lv is not null)
			{
				_levels.put(objectId, lv.Value);
				return lv.Value;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not check existing char count: " + e);
		}
		
		return 0;
	}
	
	public void setClassId(int objectId, CharacterClass classId)
	{
		_classes.put(objectId, classId);
	}
	
	public CharacterClass getClassIdById(int objectId)
	{
		if (_classes.TryGetValue(objectId, out CharacterClass classId))
		{
			return classId;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			CharacterClass? clsId = ctx.Characters.Where(c => c.Id == objectId).Select(c => (CharacterClass?)c.Class).SingleOrDefault();
			if (clsId is not null)
			{
				_classes.put(objectId, clsId.Value);
				return clsId.Value;
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
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int? dbClanId = ctx.Characters.Where(c => c.Id == objectId).Select(c => c.ClanId).SingleOrDefault();
			if (dbClanId is not null)
			{
				_clans.put(objectId, dbClanId ?? 0);
				return dbClanId ?? 0;
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
	
	public void setFriendMemo(int charId, int friendId, string memo)
	{
		Map<int, string> memos = _memos.get(charId);
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
		string text = memo.ToLower();
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
		Map<int, string> memos = _memos.get(charId);
		if (memos == null)
		{
			return;
		}
		
		memos.remove(friendId);
	}
	
	public string getFriendMemo(int charId, int friendId)
	{
		Map<int, string>? memos = _memos.get(charId);
		if (memos == null)
		{
			memos = new();
			_memos.put(charId, memos);
		}
		else if (memos.TryGetValue(friendId, out string? value))
		{
			return value;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			string memo = ctx.CharacterFriends.Where(cf => cf.CharacterId == charId && cf.FriendId == friendId)
				.Select(cf => cf.Memo).SingleOrDefault() ?? string.Empty;

			memos.put(friendId, memo);
            return memo;
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error occurred while retrieving memo: " + e);
		}
		
		// Prevent searching again.
		memos.put(friendId, string.Empty);
		return string.Empty;
	}
	
	public DateTime? getCharacterCreationDate(int objectId)
	{
		if (_creationDates.TryGetValue(objectId, out DateTime date))
			return date;
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			DateTime? createDate = ctx.Characters.Where(c => c.Id == objectId).Select(c => (DateTime?)c.Created)
				.SingleOrDefault();
			
			if (createDate is not null)
			{
				_creationDates.put(objectId, createDate.Value);
				return createDate;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not retrieve character creation date: " + e);
		}
		return null;
	}
	
	public void setLastAccess(int objectId, DateTime lastAccess)
	{
		_lastAccess.put(objectId, lastAccess);
	}
	
	public TimeSpan getLastAccessDelay(int objectId)
	{
		if (_lastAccess.TryGetValue(objectId, out DateTime lastAccess))
		{
			DateTime currentTime = DateTime.UtcNow;
			TimeSpan timeDifference = currentTime - lastAccess;
			return timeDifference;
		}
		
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			DateTime? dbLastAccess = ctx.Characters.Where(c => c.Id == objectId).Select(c => c.LastAccess)
				.SingleOrDefault();

			if (dbLastAccess is not null)
			{
				_lastAccess.put(objectId, dbLastAccess.Value);
				DateTime currentTime = DateTime.UtcNow;
				return currentTime - dbLastAccess.Value;
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Could not retrieve lastAccess timestamp: " + e);
		}
		
		return TimeSpan.Zero;
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