using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author NviX
 */
public class RankManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RankManager));
	
	public static readonly TimeSpan TIME_LIMIT = TimeSpan.FromDays(30);
	public static readonly DateTime CURRENT_TIME = DateTime.UtcNow;
	public const int PLAYER_LIMIT = 500;
	
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
			var query = ctx.Characters
				.Where(c => CURRENT_TIME - c.LastAccess < TIME_LIMIT && c.AccessLevel == 0 && c.Level >= 40)
				.OrderByDescending(c => c.Exp).ThenByDescending(c => c.OnlineTime).Take(PLAYER_LIMIT);
			
			int i = 1;
			foreach (Character character in query)
			{
				int charId = character.Id;
				CharacterClass classId = character.Class.GetRootClass(); 
				Race race = character.Class.GetRace();

				StatSet player = new StatSet();
				player.set("charId", charId);
				player.set("name", character.Name);
				player.set("level", character.Level);
				player.set("classId", (int)classId);
				player.set("race", (int)race);
					
				loadRaceRank(charId, race, player);
				loadClassRank(charId, classId, player);
				int? clanId = character.ClanId;
				if (clanId != 0)
				{
					player.set("clanName", ClanTable.getInstance().getClan(clanId.Value).getName());
				}
				else
				{
					player.set("clanName", "");
				}
					
				_mainList.put(i, player);
				i++;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not load chars total rank data: " + this + " - " + e);
		}
		
		// load olympiad data.
		try
		{
			using GameServerDbContext ctx = new();
			var query = (from n in ctx.OlympiadNobles
				from c in ctx.Characters
				where c.Id == n.CharacterId
				orderby n.OlympiadPoints descending
				select new
				{
					c.Id,
					c.Name,
					c.Level,
					c.Class,
					c.ClanId,
					n.OlympiadPoints,
					n.CompetitionsWon,
					n.CompetitionsLost
				}).Take(PLAYER_LIMIT);
				
			int i = 1;
			foreach (var record in query)
			{
				StatSet player = new StatSet();
				int charId = record.Id;
				player.set("charId", charId);
				player.set("name", record.Name);
				int? clanId = record.ClanId;
				if (clanId != null)
				{
					player.set("clanName", ClanTable.getInstance().getClan(clanId.Value).getName());
				}
				else
				{
					player.set("clanName", "");
				}
				
				player.set("level", record.Level);
				CharacterClass classId = record.Class;
				player.set("classId", (int)classId);
				if (clanId != null)
				{
					player.set("clanLevel", ClanTable.getInstance().getClan(clanId.Value).getLevel());
				}
				else
				{
					player.set("clanLevel", 0);
				}
				player.set("competitions_won", record.CompetitionsWon);
				player.set("competitions_lost", record.CompetitionsLost);
				player.set("olympiad_points", record.OlympiadPoints);
					
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
		catch (Exception e)
		{
			LOGGER.Warn("Could not load olympiad total rank data: " + this + " - " + e);
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			var query = ctx.Characters
				.Where(c => CURRENT_TIME - c.LastAccess < TIME_LIMIT && c.AccessLevel == 0 && c.Level >= 40)
				.OrderByDescending(c => c.Exp).ThenByDescending(c => c.OnlineTime).Select(c => new
				{
					c.Id,
					c.Name,
					c.Level,
					c.Class,
					c.ClanId,
					c.Deaths,
					c.Kills,
					c.PvpKills
				}).Take(PLAYER_LIMIT);
			
			int i = 1;
			foreach (var record in query)
			{
				StatSet player = new StatSet();
				int charId = record.Id;
				player.set("charId", charId);
				player.set("name", record.Name);
				player.set("level", record.Level);
				player.set("classId", (int)record.Class.GetRootClass());
				Race race = record.Class.GetRace();
				player.set("race", (int)race);
				player.set("kills", record.Kills);
				player.set("deaths", record.Deaths);
				player.set("points", record.PvpKills);
				loadRaceRank(charId, race, player);
				int? clanId = record.ClanId;
				if (clanId != null)
				{
					player.set("clanName", ClanTable.getInstance().getClan(clanId.Value).getName());
				}
				else
				{
					player.set("clanName", "");
				}
					
				_mainPvpList.put(i, player);
				i++;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not load pvp total rank data: " + this + " - " + e);
		}
		
		try
		{
			using GameServerDbContext ctx = new();

			var query =
				(from p in ctx.Pets
					from c in ctx.Characters
					from pe in ctx.PetEvolves
					from item in ctx.Items
					where p.OwnerId == c.Id && pe.ItemObjectId == p.ItemObjectId && pe.ItemObjectId == item.ObjectId
					      && CURRENT_TIME - c.LastAccess < TIME_LIMIT && c.AccessLevel == 0 && p.Level >= 40
					orderby p.Exp descending, c.OnlineTime descending
					select new
					{
						CharacterId = c.Id,
						PetExp = p.Exp,
						CharacterName = c.Name,
						PetLevel = p.Level,
						c.Class,
						CharacterLevel = c.Level,
						c.ClanId,
						pe.Index,
						PetEvolveLevel = pe.Level,
						p.ItemObjectId,
						item.ItemId
					}).Take(PLAYER_LIMIT);
				
			int i = 1;
			foreach (var record in query)
			{
				StatSet pet = new StatSet();
				int controlledItemObjId = record.ItemObjectId;
				pet.set("controlledItemObjId", controlledItemObjId);
				pet.set("name", PetDataTable.getInstance().getNameByItemObjectId(controlledItemObjId));
				pet.set("ownerId", record.CharacterId);
				pet.set("owner_name", record.CharacterName);
				pet.set("owner_race", (int)record.Class.GetRace());
				pet.set("owner_level", record.CharacterLevel);
				pet.set("level", record.PetLevel);
				pet.set("evolve_level", record.PetEvolveLevel);
				pet.set("exp", record.PetExp);
				pet.set("clanName", record.ClanId != null ? ClanTable.getInstance().getClan(record.ClanId.Value).getName() : "");
				PetData petData = PetDataTable.getInstance().getPetDataByItemId(record.ItemId);
				pet.set("petType", petData.getType());
				pet.set("npcId", petData.getNpcId());
				_mainPetList.put(i++, pet);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not load pet total rank data: " + this + " - " + e);
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			var query = (from c in ctx.Characters
				from clan in ctx.Clans
				where c.Id == clan.LeaderId && c.ClanId == clan.Id && clan.DissolvingExpireTime == null
				orderby clan.Exp descending
				select new
				{
					CharacterLevel = c.Level,
					CharacterName = c.Name,
					ClanId = clan.Id,
					ClanLevel = clan.Level,
					ClanName = clan.Name,
					clan.Reputation,
					ClanExp = clan.Exp,
				}).Take(PLAYER_LIMIT);

			
			int i = 1;
			foreach (var record in query)
			{
				StatSet player = new StatSet();
				player.set("char_name", record.CharacterName);
				player.set("level", record.CharacterLevel);
				player.set("clan_level", record.ClanLevel);
				player.set("clan_name", record.ClanName);
				player.set("reputation_score", record.Reputation);
				player.set("exp", record.ClanExp);
				player.set("clan_id", record.ClanId);
				_mainClanList.put(i, player);
				i++;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Could not load clan total rank data: " + this + " - " + e);
		}
	}
	
	private void loadClassRank(int charId, CharacterClass classId, StatSet player)
	{
		try
		{
			List<CharacterClass> classes =
				EnumUtil.GetValues<CharacterClass>().Where(c => c.GetRootClass() == classId).ToList();

			using GameServerDbContext ctx = new();
			var query = ctx.Characters
				.Where(c => CURRENT_TIME - c.LastAccess < TIME_LIMIT && c.AccessLevel == 0 && c.Level >= 40 &&
				            classes.Contains(c.Class))
				.OrderByDescending(c => c.Exp).ThenByDescending(c => c.OnlineTime).Select(c => c.Id).Take(PLAYER_LIMIT);

			int i = 0;
			foreach (int id in query)
			{
				if (id == charId)
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
		catch (Exception e)
		{
			LOGGER.Warn("Could not load chars classId olympiad rank data: " + this + " - " + e);
		}
	}
	
	private void loadRaceRank(int charId, Race race, StatSet player)
	{
		try 
		{
			List<CharacterClass> classes = EnumUtil.GetValues<CharacterClass>().Where(c => c.GetRace() == race).ToList();
			
			using GameServerDbContext ctx = new();
			var query = ctx.Characters
				.Where(c => CURRENT_TIME - c.LastAccess < TIME_LIMIT && c.AccessLevel == 0 && c.Level >= 40 &&
				            classes.Contains(c.Class))
				.OrderByDescending(c => c.Exp).ThenByDescending(c => c.OnlineTime).Select(c => c.Id).Take(PLAYER_LIMIT);

			int i = 0;
			foreach (int id in query)
			{
				if (id == charId)
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
		catch (Exception e)
		{
			LOGGER.Error("Could not load chars race rank data: " + this + " - " + e);
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