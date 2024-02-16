using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author NviX
 */
public class RankManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RankManager));
	
	public static readonly long TIME_LIMIT = 2592000000L; // 30 days in milliseconds
	public static readonly long CURRENT_TIME = System.currentTimeMillis();
	public const int PLAYER_LIMIT = 500;
	
	private static readonly string SELECT_CHARACTERS = "SELECT charId,char_name,level,race,base_class, clanid FROM characters WHERE (" + CURRENT_TIME + " - cast(lastAccess as signed) < " + TIME_LIMIT + ") AND accesslevel = 0 AND level > 39 ORDER BY exp DESC, onlinetime DESC LIMIT " + PLAYER_LIMIT;
	private static readonly string SELECT_CHARACTERS_PVP = "SELECT charId,char_name,level,race,base_class, clanid, deaths, kills, pvpkills FROM characters WHERE (" + CURRENT_TIME + " - cast(lastAccess as signed) < " + TIME_LIMIT + ") AND accesslevel = 0 AND level > 39 ORDER BY kills DESC, onlinetime DESC LIMIT " + PLAYER_LIMIT;
	private static readonly string SELECT_CHARACTERS_BY_RACE = "SELECT charId FROM characters WHERE (" + CURRENT_TIME + " - cast(lastAccess as signed) < " + TIME_LIMIT + ") AND accesslevel = 0 AND level > 39 AND race = ? ORDER BY exp DESC, onlinetime DESC LIMIT " + PLAYER_LIMIT;
	private static readonly string SELECT_PETS = "SELECT characters.charId, pets.exp, characters.char_name, pets.level as petLevel, characters.race as char_race, characters.level as char_level, characters.clanId, pet_evolves.index, pet_evolves.level as evolveLevel, pets.item_obj_id, item_id FROM characters, items, pets, pet_evolves WHERE pets.ownerId = characters.charId AND pet_evolves.itemObjId = items.object_id AND pet_evolves.itemObjId = pets.item_obj_id AND (" + CURRENT_TIME + " - cast(characters.lastAccess as signed) < " + TIME_LIMIT + ") AND characters.accesslevel = 0 AND pets.level > 39 ORDER BY pets.exp DESC, characters.onlinetime DESC LIMIT " + PLAYER_LIMIT;
	private static readonly string SELECT_CLANS = "SELECT characters.level, characters.char_name, clan_data.clan_id, clan_data.clan_level, clan_data.clan_name, clan_data.reputation_score, clan_data.exp FROM characters, clan_data WHERE characters.charId = clan_data.leader_id AND characters.clanid = clan_data.clan_id AND dissolving_expiry_time = 0 ORDER BY exp DESC LIMIT " + PLAYER_LIMIT;
	
	private static readonly string GET_CURRENT_CYCLE_DATA = "SELECT characters.char_name, characters.level, characters.base_class, characters.clanid, olympiad_nobles.charId, olympiad_nobles.olympiad_points, olympiad_nobles.competitions_won, olympiad_nobles.competitions_lost FROM characters, olympiad_nobles WHERE characters.charId = olympiad_nobles.charId ORDER BY olympiad_nobles.olympiad_points DESC LIMIT " + PLAYER_LIMIT;
	private static readonly string GET_CHARACTERS_BY_CLASS = "SELECT charId FROM characters WHERE (" + CURRENT_TIME + " - cast(lastAccess as signed) < " + TIME_LIMIT + ") AND accesslevel = 0 AND level > 39 AND characters.base_class = ? ORDER BY exp DESC, onlinetime DESC LIMIT " + PLAYER_LIMIT;
	
	private readonly Map<int, StatSet> _mainList = new();
	private Map<int, StatSet> _snapshotList = new();
	private readonly Map<int, StatSet> _mainOlyList = new();
	private Map<int, StatSet> _snapshotOlyList = new();
	private readonly Map<int, StatSet> _mainPvpList = new();
	private Map<int, StatSet> _snapshotPvpList = new();
	private readonly Map<int, StatSet> _mainPetList = new();
	private Map<int, StatSet> _snapshotPetList = new();
	private readonly Map<int, StatSet> _mainClanList = new();
	private Map<int, StatSet> _snapshotClanList = new();
	
	protected RankManager()
	{
		ThreadPool.scheduleAtFixedRate(update, 0, 1800000);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	private void update()
	{
		// Load charIds All
		_snapshotList = _mainList;
		_mainList.clear();
		_snapshotOlyList = _mainOlyList;
		_mainOlyList.clear();
		_snapshotPvpList = _mainPvpList;
		_mainPvpList.clear();
		_snapshotPetList = _mainPetList;
		_mainPetList.clear();
		_snapshotClanList = _mainClanList;
		_mainClanList.clear();
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(SELECT_CHARACTERS);

			{
				ResultSet rset = statement.executeQuery();
				int i = 1;
				while (rset.next())
				{
					StatSet player = new StatSet();
					int charId = rset.getInt("charId");
					int classId = rset.getInt("base_class");
					player.set("charId", charId);
					player.set("name", rset.getString("char_name"));
					player.set("level", rset.getInt("level"));
					player.set("classId", rset.getInt("base_class"));
					int race = rset.getInt("race");
					player.set("race", race);
					
					loadRaceRank(charId, race, player);
					loadClassRank(charId, classId, player);
					int clanId = rset.getInt("clanid");
					if (clanId > 0)
					{
						player.set("clanName", ClanTable.getInstance().getClan(clanId).getName());
					}
					else
					{
						player.set("clanName", "");
					}
					
					_mainList.put(i, player);
					i++;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not load chars total rank data: " + this + " - " + e);
		}
		
		// load olympiad data.
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(GET_CURRENT_CYCLE_DATA);

			{
				ResultSet rset = statement.executeQuery();
				int i = 1;
				while (rset.next())
				{
					StatSet player = new StatSet();
					int charId = rset.getInt("charId");
					player.set("charId", charId);
					player.set("name", rset.getString("char_name"));
					int clanId = rset.getInt("clanid");
					if (clanId > 0)
					{
						player.set("clanName", ClanTable.getInstance().getClan(clanId).getName());
					}
					else
					{
						player.set("clanName", "");
					}
					player.set("level", rset.getInt("level"));
					int classId = rset.getInt("base_class");
					player.set("classId", classId);
					if (clanId > 0)
					{
						player.set("clanLevel", ClanTable.getInstance().getClan(clanId).getLevel());
					}
					else
					{
						player.set("clanLevel", 0);
					}
					player.set("competitions_won", rset.getInt("competitions_won"));
					player.set("competitions_lost", rset.getInt("competitions_lost"));
					player.set("olympiad_points", rset.getInt("olympiad_points"));
					
					if (Hero.getInstance().getCompleteHeroes().containsKey(charId))
					{
						StatSet hero = Hero.getInstance().getCompleteHeroes().get(charId);
						player.set("count", hero.getInt("count", 0));
						player.set("legend_count", hero.getInt("legend_count", 0));
					}
					else
					{
						player.set("count", 0);
						player.set("legend_count", 0);
					}
					
					loadClassRank(charId, classId, player);
					
					_mainOlyList.put(i, player);
					i++;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not load olympiad total rank data: " + this + " - " + e);
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(SELECT_CHARACTERS_PVP);

			{
				ResultSet rset = statement.executeQuery();
				int i = 1;
				while (rset.next())
				{
					StatSet player = new StatSet();
					int charId = rset.getInt("charId");
					player.set("charId", charId);
					player.set("name", rset.getString("char_name"));
					player.set("level", rset.getInt("level"));
					player.set("classId", rset.getInt("base_class"));
					int race = rset.getInt("race");
					player.set("race", race);
					player.set("kills", rset.getInt("kills"));
					player.set("deaths", rset.getInt("deaths"));
					player.set("points", rset.getInt("pvpkills"));
					loadRaceRank(charId, race, player);
					int clanId = rset.getInt("clanid");
					if (clanId > 0)
					{
						player.set("clanName", ClanTable.getInstance().getClan(clanId).getName());
					}
					else
					{
						player.set("clanName", "");
					}
					
					_mainPvpList.put(i, player);
					i++;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not load pvp total rank data: " + this + " - " + e);
		}
		
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(SELECT_PETS);

			{
				ResultSet rset = statement.executeQuery();
				int i = 1;
				while (rset.next())
				{
					StatSet pet = new StatSet();
					int controlledItemObjId = rset.getInt("item_obj_id");
					pet.set("controlledItemObjId", controlledItemObjId);
					pet.set("name", PetDataTable.getInstance().getNameByItemObjectId(controlledItemObjId));
					pet.set("ownerId", rset.getInt("charId"));
					pet.set("owner_name", rset.getString("char_name"));
					pet.set("owner_race", rset.getString("char_race"));
					pet.set("owner_level", rset.getInt("char_level"));
					pet.set("level", rset.getInt("petLevel"));
					pet.set("evolve_level", rset.getInt("evolveLevel"));
					pet.set("exp", rset.getLong("exp"));
					pet.set("clanName", rset.getInt("clanid") > 0 ? ClanTable.getInstance().getClan(rset.getInt("clanid")).getName() : "");
					PetData petData = PetDataTable.getInstance().getPetDataByItemId(rset.getInt("item_id"));
					pet.set("petType", petData.getType());
					pet.set("npcId", petData.getNpcId());
					_mainPetList.put(i++, pet);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not load pet total rank data: " + this + " - " + e);
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(SELECT_CLANS);

			{
				ResultSet rset = statement.executeQuery();
				int i = 1;
				while (rset.next())
				{
					StatSet player = new StatSet();
					player.set("char_name", rset.getString("char_name"));
					player.set("level", rset.getInt("level"));
					player.set("clan_level", rset.getInt("clan_level"));
					player.set("clan_name", rset.getString("clan_name"));
					player.set("reputation_score", rset.getInt("reputation_score"));
					player.set("exp", rset.getLong("exp"));
					player.set("clan_id", rset.getInt("clan_id"));
					_mainClanList.put(i, player);
					i++;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not load clan total rank data: " + this + " - " + e);
		}
	}
	
	private void loadClassRank(int charId, int classId, StatSet player)
	{
		try
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(GET_CHARACTERS_BY_CLASS);
			ps.setInt(1, classId);

			{
				ResultSet rset = ps.executeQuery();
				int i = 0;
				while (rset.next())
				{
					if (rset.getInt("charId") == charId)
					{
						player.set("classRank", i + 1);
					}
					i++;
				}
				if (i == 0)
				{
					player.set("classRank", 0);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not load chars classId olympiad rank data: " + this + " - " + e);
		}
	}
	
	private void loadRaceRank(int charId, int race, StatSet player)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement ps = con.prepareStatement(SELECT_CHARACTERS_BY_RACE);
			ps.setInt(1, race);
			{
				ResultSet rset = ps.executeQuery()
				int i = 0;
				while (rset.next())
				{
					if (rset.getInt("charId") == charId)
					{
						player.set("raceRank", i + 1);
					}
					i++;
				}
				if (i == 0)
				{
					player.set("raceRank", 0);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not load chars race rank data: " + this + " - " + e);
		}
	}
	
	public Map<int, StatSet> getRankList()
	{
		return _mainList;
	}
	
	public Map<int, StatSet> getSnapshotList()
	{
		return _snapshotList;
	}
	
	public Map<int, StatSet> getOlyRankList()
	{
		return _mainOlyList;
	}
	
	public Map<int, StatSet> getSnapshotOlyList()
	{
		return _snapshotOlyList;
	}
	
	public Map<int, StatSet> getPvpRankList()
	{
		return _mainPvpList;
	}
	
	public Map<int, StatSet> getSnapshotPvpRankList()
	{
		return _snapshotPvpList;
	}
	
	public Map<int, StatSet> getPetRankList()
	{
		return _mainPetList;
	}
	
	public Map<int, StatSet> getSnapshotPetRankList()
	{
		return _snapshotPetList;
	}
	
	public Map<int, StatSet> getClanRankList()
	{
		return _mainClanList;
	}
	
	public Map<int, StatSet> getSnapshotClanRankList()
	{
		return _snapshotClanList;
	}
	
	public int getPlayerGlobalRank(Player player)
	{
		int playerOid = player.getObjectId();
		foreach (var entry in _mainList)
		{
			StatSet stats = entry.Value;
			if (stats.getInt("charId") != playerOid)
			{
				continue;
			}
			return entry.Key;
		}
		return 0;
	}
	
	public int getPlayerRaceRank(Player player)
	{
		int playerOid = player.getObjectId();
		foreach (StatSet stats in _mainList.values())
		{
			if (stats.getInt("charId") != playerOid)
			{
				continue;
			}
			return stats.getInt("raceRank");
		}
		return 0;
	}
	
	public int getPlayerClassRank(Player player)
	{
		int playerOid = player.getObjectId();
		foreach (StatSet stats in _mainList.values())
		{
			if (stats.getInt("charId") != playerOid)
			{
				continue;
			}
			return stats.getInt("classRank");
		}
		return 0;
	}
	
	public ICollection<int> getTop50()
	{
		List<int> result = new();
		for (int i = 1; i <= 50; i++)
		{
			StatSet rank = _mainList.get(i);
			if (rank == null)
			{
				break;
			}
			result.add(rank.getInt("charId"));
		}
		return result;
	}
	
	public static RankManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RankManager INSTANCE = new RankManager();
	}
}