using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.ChatHandlers;

/**
 * World chat handler.
 * @author UnAfraid
 */
public class ChatWorld: IChatHandler
{
	private static readonly Map<int, DateTime> REUSE = new();

	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.WORLD,
	};

	public void handleChat(ChatType type, Player activeChar, string target, string text, bool shareLocation)
	{
		if (!Config.ENABLE_WORLD_CHAT)
		{
			return;
		}

		DateTime now = DateTime.UtcNow;
		if (REUSE.Count != 0)
		{
			List<int> expired = REUSE.Where(r => r.Value <= now).Select(r => r.Key).ToList();
			foreach (int id in expired)
				REUSE.remove(id);
		}

		if (activeChar.getLevel() < Config.WORLD_CHAT_MIN_LEVEL)
		{
			SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_CAN_USE_WORLD_CHAT_FROM_LV_S1);
			msg.Params.addInt(Config.WORLD_CHAT_MIN_LEVEL);
			activeChar.sendPacket(msg);
		}
		else if (activeChar.isChatBanned() && Config.BAN_CHAT_CHANNELS.Contains(type))
		{
			activeChar.sendPacket(SystemMessageId.IF_YOU_TRY_TO_CHAT_BEFORE_THE_PROHIBITION_IS_REMOVED_THE_PROHIBITION_TIME_WILL_INCREASE_EVEN_FURTHER_S1_SEC_OF_PROHIBITION_IS_LEFT);
		}
		else if (Config.JAIL_DISABLE_CHAT && activeChar.isJailed() && !activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
		{
			activeChar.sendPacket(SystemMessageId.CHATTING_IS_CURRENTLY_PROHIBITED);
		}
		else if (activeChar.getWorldChatUsed() >= activeChar.getWorldChatPoints())
		{
			activeChar.sendPacket(SystemMessageId.YOU_HAVE_SPENT_YOUR_WORLD_CHAT_QUOTA_FOR_THE_DAY_IT_IS_RESET_DAILY_AT_7_A_M);
		}
		else if (shareLocation && activeChar.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) < Config.SHARING_LOCATION_COST)
		{
			activeChar.sendPacket(SystemMessageId.THERE_ARE_NOT_ENOUGH_L_COINS);
		}
		else if (shareLocation && (activeChar.getMovieHolder() != null || activeChar.isFishing() || activeChar.isInInstance() || activeChar.isOnEvent() || activeChar.isInOlympiadMode() || activeChar.inObserverMode() || activeChar.isInTraingCamp() || activeChar.isInTimedHuntingZone() || activeChar.isInsideZone(ZoneId.SIEGE)))
		{
			activeChar.sendPacket(SystemMessageId.LOCATION_CANNOT_BE_SHARED_SINCE_THE_CONDITIONS_ARE_NOT_MET);
		}
		else
		{
			// Verify if player is not spaming.
			if (Config.WORLD_CHAT_INTERVAL > TimeSpan.Zero)
			{
				if (REUSE.TryGetValue(activeChar.ObjectId, out DateTime instant) && instant > now)
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_HAVE_S1_SEC_UNTIL_YOU_ARE_ABLE_TO_USE_WORLD_CHAT);
					msg.Params.addInt((int)(instant - now).TotalSeconds);
					activeChar.sendPacket(msg);
					return;

				}
			}

			if (shareLocation)
			{
				activeChar.destroyItemByItemId("Shared Location", Inventory.LCOIN_ID, Config.SHARING_LOCATION_COST, activeChar, true);
			}

			CreatureSayPacket cs = new CreatureSayPacket(activeChar, type, activeChar.getName(), text, shareLocation);
			if (Config.FACTION_SYSTEM_ENABLED && Config.FACTION_SPECIFIC_CHAT)
			{
				if (activeChar.isGood())
				{
					foreach (Player player in World.getInstance().getAllGoodPlayers())
					{
						if (activeChar.isNotBlocked(player))
						{
							player.sendPacket(cs);
						}
					}
				}
				if (activeChar.isEvil())
				{
					foreach (Player player in World.getInstance().getAllEvilPlayers())
					{
						if (activeChar.isNotBlocked(player))
						{
							player.sendPacket(cs);
						}
					}
				}
			}
			else
			{
				foreach (Player player in World.getInstance().getPlayers())
				{
					if (activeChar.isNotBlocked(player))
					{
						player.sendPacket(cs);
					}
				}
			}

			activeChar.setWorldChatUsed(activeChar.getWorldChatUsed() + 1);
			activeChar.sendPacket(new ExWorldCharCntPacket(activeChar));
			if (Config.WORLD_CHAT_INTERVAL > TimeSpan.Zero)
			{
				REUSE.put(activeChar.ObjectId, now + Config.WORLD_CHAT_INTERVAL);
			}
		}
	}

	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}