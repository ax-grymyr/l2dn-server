using L2Dn.GameServer.Db;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius (reworked from L2J IdFactory)
 */
public class IdManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(IdManager));
	
	private const int FIRST_OID = 0x10000000;
	private const int LAST_OID = 0x7FFFFFFF;
	
	// There are 1,879,048,192 numbers available for object ids.
	// Whole bitmap takes exactly 224 MB of memory, that's not a large number.
	private const int FREE_OBJECT_ID_SIZE = LAST_OID - FIRST_OID + 1;
	private const int IdBitSetCount = FREE_OBJECT_ID_SIZE / IdBitSet.BitCount;
	
	private readonly IdBitSet[] _bitSets = new IdBitSet[IdBitSetCount];
	
	public IdManager()
	{
		// Update characters online status.
		try 
		{
			using GameServerDbContext ctx = new();

			// TODO: this is not right place for this query
			ctx.Characters.ExecuteUpdate(s => s.SetProperty(c => c.OnlineStatus, CharacterOnlineStatus.Offline));

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

				DateTime cleanupStart = DateTime.UtcNow;
				int cleanCount = 0;

				// TODO: configure cascade delete and cascade update in the DB context instead of this shit
				
				// Characters
				// cleanCount += ctx.AccountVariables.Where(r => !ctx.AccountRefs.Select(a => a.Id).Contains(r.AccountId))
				// 	.ExecuteDelete();
				
				// cleanCount += statement.executeUpdate("DELETE FROM account_gsdata WHERE account_gsdata.account_name NOT IN (SELECT account_name FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_contacts WHERE character_contacts.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_contacts WHERE character_contacts.contactId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_friends WHERE character_friends.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_friends WHERE character_friends.friendId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_hennas WHERE character_hennas.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_macroses WHERE character_macroses.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_quests WHERE character_quests.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_recipebook WHERE character_recipebook.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_recipeshoplist WHERE character_recipeshoplist.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_shortcuts WHERE character_shortcuts.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_skills WHERE character_skills.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_skills_save WHERE character_skills_save.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_spirits WHERE character_spirits.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_subclasses WHERE character_subclasses.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_instance_time WHERE character_instance_time.charId NOT IN (SELECT charId FROM characters);");
				//
				// // Items
				// cleanCount += statement.executeUpdate("DELETE FROM items WHERE items.owner_id NOT IN (SELECT charId FROM characters) AND items.owner_id NOT IN (SELECT clan_id FROM clan_data) AND items.owner_id NOT IN (SELECT item_obj_id FROM pets) AND items.owner_id != -1;");
				// cleanCount += statement.executeUpdate("DELETE FROM items WHERE items.owner_id = -1 AND loc LIKE 'MAIL' AND loc_data NOT IN (SELECT messageId FROM messages WHERE senderId = -1);");
				// cleanCount += statement.executeUpdate("DELETE FROM item_auction_bid WHERE item_auction_bid.playerObjId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM item_variations WHERE item_variations.itemId NOT IN (SELECT object_id FROM items);");
				// cleanCount += statement.executeUpdate("DELETE FROM item_elementals WHERE item_elementals.itemId NOT IN (SELECT object_id FROM items);");
				// cleanCount += statement.executeUpdate("DELETE FROM item_special_abilities WHERE item_special_abilities.objectId NOT IN (SELECT object_id FROM items);");
				// cleanCount += statement.executeUpdate("DELETE FROM item_variables WHERE item_variables.id NOT IN (SELECT object_id FROM items);");
				//
				// // Misc
				// cleanCount += statement.executeUpdate("DELETE FROM cursed_weapons WHERE cursed_weapons.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM heroes WHERE heroes.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM olympiad_nobles WHERE olympiad_nobles.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM olympiad_nobles_eom WHERE olympiad_nobles_eom.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM pets WHERE pets.item_obj_id NOT IN (SELECT object_id FROM items);");
				// cleanCount += statement.executeUpdate("DELETE FROM merchant_lease WHERE merchant_lease.player_id NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_reco_bonus WHERE character_reco_bonus.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM clan_data WHERE clan_data.leader_id NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM clan_data WHERE clan_data.clan_id NOT IN (SELECT clanid FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM olympiad_fights WHERE olympiad_fights.charOneId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM olympiad_fights WHERE olympiad_fights.charTwoId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM heroes_diary WHERE heroes_diary.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_offline_trade WHERE character_offline_trade.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_offline_trade_items WHERE character_offline_trade_items.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_tpbookmark WHERE character_tpbookmark.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_variables WHERE character_variables.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM character_revenge_history WHERE character_revenge_history.charId NOT IN (SELECT charId FROM characters);");
				// cleanCount += statement.executeUpdate("DELETE FROM bot_reported_char_data WHERE bot_reported_char_data.botId NOT IN (SELECT charId FROM characters);");
				//
				// // Clan
				// cleanCount += statement.executeUpdate("DELETE FROM clan_privs WHERE clan_privs.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				// cleanCount += statement.executeUpdate("DELETE FROM clan_skills WHERE clan_skills.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				// cleanCount += statement.executeUpdate("DELETE FROM clan_subpledges WHERE clan_subpledges.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				// cleanCount += statement.executeUpdate("DELETE FROM clan_wars WHERE clan_wars.clan1 NOT IN (SELECT clan_id FROM clan_data);");
				// cleanCount += statement.executeUpdate("DELETE FROM clan_wars WHERE clan_wars.clan2 NOT IN (SELECT clan_id FROM clan_data);");
				// cleanCount += statement.executeUpdate("DELETE FROM siege_clans WHERE siege_clans.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				// cleanCount += statement.executeUpdate("DELETE FROM clan_notices WHERE clan_notices.clan_id NOT IN (SELECT clan_id FROM clan_data);");
				// cleanCount += statement.executeUpdate("DELETE FROM auction_bid WHERE auction_bid.bidderId NOT IN (SELECT clan_id FROM clan_data);");
				//
				// // Forums
				// cleanCount += statement.executeUpdate("DELETE FROM forums WHERE forums.forum_owner_id NOT IN (SELECT clan_id FROM clan_data) AND forums.forum_parent=2;");
				// cleanCount += statement.executeUpdate("DELETE FROM forums WHERE forums.forum_owner_id NOT IN (SELECT charId FROM characters) AND forums.forum_parent=3;");
				// cleanCount += statement.executeUpdate("DELETE FROM posts WHERE posts.post_forum_id NOT IN (SELECT forum_id FROM forums);");
				// cleanCount += statement.executeUpdate("DELETE FROM topic WHERE topic.topic_forum_id NOT IN (SELECT forum_id FROM forums);");
				
				// Update needed items after cleaning has taken place.
				// statement.executeUpdate("UPDATE clan_data SET auction_bid_at = 0 WHERE auction_bid_at NOT IN (SELECT auctionId FROM auction_bid);");
				// statement.executeUpdate("UPDATE clan_data SET new_leader_id = 0 WHERE new_leader_id <> 0 AND new_leader_id NOT IN (SELECT charId FROM characters);");
				// statement.executeUpdate("UPDATE clan_subpledges SET leader_id=0 WHERE clan_subpledges.leader_id NOT IN (SELECT charId FROM characters) AND leader_id > 0;");
				// statement.executeUpdate("UPDATE castle SET side='NEUTRAL' WHERE castle.id NOT IN (SELECT hasCastle FROM clan_data);");
				// statement.executeUpdate("UPDATE characters SET clanid=0, clan_privs=0, wantspeace=0, subpledge=0, lvl_joined_academy=0, apprentice=0, sponsor=0, clan_join_expiry_time=0, clan_create_expiry_time=0 WHERE characters.clanid > 0 AND characters.clanid NOT IN (SELECT clan_id FROM clan_data);");
				// statement.executeUpdate("UPDATE fort SET owner=0 WHERE owner NOT IN (SELECT clan_id FROM clan_data);");
				
				LOGGER.Info("IdManager: Cleaned " + cleanCount + " elements from database in " + ((DateTime.UtcNow - cleanupStart) / 1000) + " seconds.");
			}
			catch (Exception e)
			{
				LOGGER.Error("IdManager: Could not clean up database: " + e);
			}
		}
		
		// Cleanup timestamps.
		try
		{
			using GameServerDbContext ctx = new();

			int cleanCount = 0;
			cleanCount += ctx.CharacterInstances.Where(r => r.Time <= DateTime.UtcNow).ExecuteDelete();
			cleanCount += ctx.CharacterSkillReuses.Where(r => r.RestoreType == 1 && r.SysTime <= DateTime.UtcNow)
				.ExecuteDelete();
			
			LOGGER.Info("IdManager: Cleaned " + cleanCount + " expired timestamps from database.");
		}
		catch (Exception e)
		{
			LOGGER.Warn("IdManager: Could not clean expired timestamps from database. " + e);
		}
		
		// Initialize.
		try
		{
			// Collect already used ids.
			using GameServerDbContext ctx = new();
			List<int> usedIds = ctx.Characters.Select(c => c.Id).Concat(ctx.Items.Select(i => i.ObjectId))
				.Concat(ctx.Clans.Select(c => c.Id)).Concat(ctx.ItemsOnGround.Select(c => c.ObjectId))
				.Concat(ctx.MailMessages.Select(c => c.MessageId)).ToList();
			
			// Register used ids.
			foreach (int usedObjectId in usedIds)
			{
				if (usedObjectId < FIRST_OID)
				{
					LOGGER.Error("IdManager: Object ID " + usedObjectId + " in DB is less than minimum ID of " + FIRST_OID);
					continue;
				}

				ReserveId(usedObjectId);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("IdManager: Could not initialize properly: " + e);
		}
		
		// TODO Schedule cleanup task

		int freeIds = FREE_OBJECT_ID_SIZE - _bitSets.Sum(x => x.SetBitCount);
		LOGGER.Info("IdManager: " + freeIds + " id's available.");
	}
	
	public void releaseId(int id)
	{
		int index = id - FIRST_OID;
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(id));

		int bitSetIndex = index >> 23;
		const int mask = (1 << 23) - 1;
		_bitSets[bitSetIndex].ReleaseId(index & mask);
	}
	
	public int getNextId()
	{
		for (int bitSetIndex = 0; bitSetIndex < _bitSets.Length; bitSetIndex++)
		{
			int? id = _bitSets[bitSetIndex].ReserveId();
			if (id != null)
				return (bitSetIndex << 23) + id.Value + FIRST_OID;
		}

		throw new InvalidOperationException("IdManager: Ran out of valid ids.");
	}

	private void ReserveId(int id)
	{
		int index = id - FIRST_OID;
		if (index < 0)
			throw new ArgumentOutOfRangeException(nameof(id));

		int bitSetIndex = index >> 23;
		const int mask = (1 << 23) - 1;
		_bitSets[bitSetIndex].ReserveId(index & mask);
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