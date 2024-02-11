using System.Runtime.CompilerServices;
using System.Text;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius (reworked from L2J IdFactory)
 */
public class IdManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(IdManager));
	
	//@formatter:off
	private static readonly string[][] ID_EXTRACTS =
	{
		["characters","charId"],
		["items","object_id"],
		["clan_data","clan_id"],
		["itemsonground","object_id"],
		["messages","messageId"]
	};
	//@formatter:on
	
	private static readonly string[] TIMESTAMPS_CLEAN =
	{
		"DELETE FROM character_instance_time WHERE time <= ?",
		"DELETE FROM character_skills_save WHERE restore_type = 1 AND systime <= ?"
	};
	
	private const int FIRST_OID = 0x10000000;
	private const int LAST_OID = 0x7FFFFFFF;
	private const int FREE_OBJECT_ID_SIZE = LAST_OID - FIRST_OID;
	
	private BitSet _freeIds;
	private AtomicInteger _freeIdCount;
	private AtomicInteger _nextFreeId;
	
	public IdManager()
	{
		// Update characters online status.
		try 
		{
			using GameServerDbContext ctx = new();
			Statement statement = con.createStatement();
			statement.executeUpdate("UPDATE characters SET online = 0");
			LOGGER.Info("Updated characters online status.");
		}
		catch (Exception e)
		{
			LOGGER.Warn("IdManager: Could not update characters online status: " + e);
		}
		
		// Cleanup database.
		if (Config.DATABASE_CLEAN_UP)
		{
			try 
			{
				using GameServerDbContext ctx = new();
				Statement statement = con.createStatement();
				long cleanupStart = System.currentTimeMillis();
				int cleanCount = 0;
				
				// Characters
				cleanCount += statement.executeUpdate("DELETE FROM account_gsdata WHERE account_gsdata.account_name NOT IN (SELECT account_name FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_contacts WHERE character_contacts.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_contacts WHERE character_contacts.contactId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_friends WHERE character_friends.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_friends WHERE character_friends.friendId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_hennas WHERE character_hennas.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_macroses WHERE character_macroses.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_quests WHERE character_quests.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_recipebook WHERE character_recipebook.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_recipeshoplist WHERE character_recipeshoplist.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_shortcuts WHERE character_shortcuts.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_skills WHERE character_skills.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_skills_save WHERE character_skills_save.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_spirits WHERE character_spirits.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_subclasses WHERE character_subclasses.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_instance_time WHERE character_instance_time.charId NOT IN (SELECT charId FROM characters);");
				
				// Items
				cleanCount += statement.executeUpdate("DELETE FROM items WHERE items.owner_id NOT IN (SELECT charId FROM characters) AND items.owner_id NOT IN (SELECT clan_id FROM clan_data) AND items.owner_id NOT IN (SELECT item_obj_id FROM pets) AND items.owner_id != -1;");
				cleanCount += statement.executeUpdate("DELETE FROM items WHERE items.owner_id = -1 AND loc LIKE 'MAIL' AND loc_data NOT IN (SELECT messageId FROM messages WHERE senderId = -1);");
				cleanCount += statement.executeUpdate("DELETE FROM item_auction_bid WHERE item_auction_bid.playerObjId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM item_variations WHERE item_variations.itemId NOT IN (SELECT object_id FROM items);");
				cleanCount += statement.executeUpdate("DELETE FROM item_elementals WHERE item_elementals.itemId NOT IN (SELECT object_id FROM items);");
				cleanCount += statement.executeUpdate("DELETE FROM item_special_abilities WHERE item_special_abilities.objectId NOT IN (SELECT object_id FROM items);");
				cleanCount += statement.executeUpdate("DELETE FROM item_variables WHERE item_variables.id NOT IN (SELECT object_id FROM items);");
				
				// Misc
				cleanCount += statement.executeUpdate("DELETE FROM cursed_weapons WHERE cursed_weapons.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM heroes WHERE heroes.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM olympiad_nobles WHERE olympiad_nobles.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM olympiad_nobles_eom WHERE olympiad_nobles_eom.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM pets WHERE pets.item_obj_id NOT IN (SELECT object_id FROM items);");
				cleanCount += statement.executeUpdate("DELETE FROM merchant_lease WHERE merchant_lease.player_id NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_reco_bonus WHERE character_reco_bonus.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM clan_data WHERE clan_data.leader_id NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM clan_data WHERE clan_data.clan_id NOT IN (SELECT clanid FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM olympiad_fights WHERE olympiad_fights.charOneId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM olympiad_fights WHERE olympiad_fights.charTwoId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM heroes_diary WHERE heroes_diary.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_offline_trade WHERE character_offline_trade.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_offline_trade_items WHERE character_offline_trade_items.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_tpbookmark WHERE character_tpbookmark.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_variables WHERE character_variables.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM character_revenge_history WHERE character_revenge_history.charId NOT IN (SELECT charId FROM characters);");
				cleanCount += statement.executeUpdate("DELETE FROM bot_reported_char_data WHERE bot_reported_char_data.botId NOT IN (SELECT charId FROM characters);");
				
				// Clan
				cleanCount += statement.executeUpdate("DELETE FROM clan_privs WHERE clan_privs.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				cleanCount += statement.executeUpdate("DELETE FROM clan_skills WHERE clan_skills.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				cleanCount += statement.executeUpdate("DELETE FROM clan_subpledges WHERE clan_subpledges.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				cleanCount += statement.executeUpdate("DELETE FROM clan_wars WHERE clan_wars.clan1 NOT IN (SELECT clan_id FROM clan_data);");
				cleanCount += statement.executeUpdate("DELETE FROM clan_wars WHERE clan_wars.clan2 NOT IN (SELECT clan_id FROM clan_data);");
				cleanCount += statement.executeUpdate("DELETE FROM siege_clans WHERE siege_clans.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				cleanCount += statement.executeUpdate("DELETE FROM clan_notices WHERE clan_notices.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				cleanCount += statement.executeUpdate("DELETE FROM auction_bid WHERE auction_bid.bidderId NOT IN (SELECT clan_id FROM clan_data);");
				
				// Forums
				cleanCount += statement.executeUpdate("DELETE FROM forums WHERE forums.forum_owner_id NOT IN (SELECT clan_id FROM clan_data) AND forums.forum_parent=2;");
				cleanCount += statement.executeUpdate("DELETE FROM forums WHERE forums.forum_owner_id NOT IN (SELECT charId FROM characters) AND forums.forum_parent=3;");
				cleanCount += statement.executeUpdate("DELETE FROM posts WHERE posts.post_forum_id NOT IN (SELECT forum_id FROM forums);");
				cleanCount += statement.executeUpdate("DELETE FROM topic WHERE topic.topic_forum_id NOT IN (SELECT forum_id FROM forums);");
				
				// Update needed items after cleaning has taken place.
				statement.executeUpdate("UPDATE clan_data SET auction_bid_at = 0 WHERE auction_bid_at NOT IN (SELECT auctionId FROM auction_bid);");
				statement.executeUpdate("UPDATE clan_data SET new_leader_id = 0 WHERE new_leader_id <> 0 AND new_leader_id NOT IN (SELECT charId FROM characters);");
				statement.executeUpdate("UPDATE clan_subpledges SET leader_id=0 WHERE clan_subpledges.leader_id NOT IN (SELECT charId FROM characters) AND leader_id > 0;");
				statement.executeUpdate("UPDATE castle SET side='NEUTRAL' WHERE castle.id NOT IN (SELECT hasCastle FROM clan_data);");
				statement.executeUpdate("UPDATE characters SET clanid=0, clan_privs=0, wantspeace=0, subpledge=0, lvl_joined_academy=0, apprentice=0, sponsor=0, clan_join_expiry_time=0, clan_create_expiry_time=0 WHERE characters.clanid > 0 AND characters.clanid NOT IN (SELECT clan_id FROM clan_data);");
				statement.executeUpdate("UPDATE fort SET owner=0 WHERE owner NOT IN (SELECT clan_id FROM clan_data);");
				
				LOGGER.Info("IdManager: Cleaned " + cleanCount + " elements from database in " + ((System.currentTimeMillis() - cleanupStart) / 1000) + " seconds.");
			}
			catch (Exception e)
			{
				LOGGER.Warn("IdManager: Could not clean up database: " + e);
			}
		}
		
		// Cleanup timestamps.
		try
		{
			using GameServerDbContext ctx = new();
			int cleanCount = 0;
			foreach (String line in TIMESTAMPS_CLEAN)
			{

				{
					PreparedStatement statement = con.prepareStatement(line);
					statement.setLong(1, System.currentTimeMillis());
					cleanCount += statement.executeUpdate();
				}
			}
			LOGGER.Info("IdManager: Cleaned " + cleanCount + " expired timestamps from database.");
		}
		catch (Exception e)
		{
			LOGGER.Warn("IdManager: Could not clean expired timestamps from database. " + e);
		}
		
		// Initialize.
		try
		{
			_freeIds = new BitSet(PrimeFinder.nextPrime(100000));
			_freeIds.clear();
			_freeIdCount = new AtomicInteger(FREE_OBJECT_ID_SIZE);
			
			// Collect already used ids.
			List<int> usedIds = new();
			try 
			{
				using GameServerDbContext ctx = new();
				Statement statement = con.createStatement();
				StringBuilder extractUsedObjectIdsQuery = new StringBuilder();
				foreach (String[] tblClmn in ID_EXTRACTS)
				{
					extractUsedObjectIdsQuery.Append("SELECT ");
					extractUsedObjectIdsQuery.Append(tblClmn[1]);
					extractUsedObjectIdsQuery.Append(" FROM ");
					extractUsedObjectIdsQuery.Append(tblClmn[0]);
					extractUsedObjectIdsQuery.Append(" UNION ");
				}
				
				extractUsedObjectIdsQuery.SetLength(Math.Max(extractUsedObjectIdsQuery.Length - 7, 0)); // Remove the last " UNION "

				{
					ResultSet result = statement.executeQuery(extractUsedObjectIdsQuery.ToString());
					while (result.next())
					{
						usedIds.add(result.getInt(1));
					}
				}
			}
			
			// Register used ids.
			foreach (int usedObjectId in usedIds)
			{
				int objectId = usedObjectId - FIRST_OID;
				if (objectId < 0)
				{
					LOGGER.Warn("IdManager: Object ID " + usedObjectId + " in DB is less than minimum ID of " + FIRST_OID);
					continue;
				}
				_freeIds.set(objectId);
				_freeIdCount.decrementAndGet();
			}
			
			_nextFreeId = new AtomicInteger(_freeIds.nextClearBit(0));
		}
		catch (Exception e)
		{
			LOGGER.Error("IdManager: Could not be initialized properly: " + e);
		}
		
		// Schedule increase capacity task.
		ThreadPool.scheduleAtFixedRate(() =>
		{
			if (PrimeFinder.nextPrime((usedIdCount() * 11) / 10) > _freeIds.size())
			{
				increaseBitSetCapacity();
			}
		}, 30000, 30000);
		
		LOGGER.Info("IdManager: " + _freeIds.size() + " id's available.");
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void releaseId(int objectId)
	{
		if ((objectId - FIRST_OID) > -1)
		{
			_freeIds.clear(objectId - FIRST_OID);
			_freeIdCount.incrementAndGet();
		}
		else
		{
			LOGGER.Warn("IdManager: Release objectID " + objectId + " failed (< " + FIRST_OID + ")");
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public int getNextId()
	{
		int newId = _nextFreeId.get();
		_freeIds.set(newId);
		_freeIdCount.decrementAndGet();
		
		int nextFree = _freeIds.nextClearBit(newId);
		if (nextFree < 0)
		{
			nextFree = _freeIds.nextClearBit(0);
		}
		if (nextFree < 0)
		{
			if (_freeIds.size() < FREE_OBJECT_ID_SIZE)
			{
				increaseBitSetCapacity();
			}
			else
			{
				throw new NullPointerException("IdManager: Ran out of valid ids.");
			}
		}
		_nextFreeId.set(nextFree);
		
		return newId + FIRST_OID;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	private void increaseBitSetCapacity()
	{
		BitSet newBitSet = new BitSet(PrimeFinder.nextPrime((usedIdCount() * 11) / 10));
		newBitSet.or(_freeIds);
		_freeIds = newBitSet;
	}
	
	private int usedIdCount()
	{
		return _freeIdCount.get() - FIRST_OID;
	}
	
	public int size()
	{
		return _freeIdCount.get();
	}
	
	public static IdManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly IdManager INSTANCE = new IdManager();
	}
}