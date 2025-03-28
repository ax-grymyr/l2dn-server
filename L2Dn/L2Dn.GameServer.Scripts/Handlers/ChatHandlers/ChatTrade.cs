using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.ChatHandlers;

/**
 * Trade chat handler.
 * @author durgus
 */
public class ChatTrade: IChatHandler
{
	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.TRADE,
	};

	public void handleChat(ChatType type, Player activeChar, string target, string text, bool shareLocation)
	{
		if (activeChar.isChatBanned() && Config.General.BAN_CHAT_CHANNELS.Contains(type))
		{
			activeChar.sendPacket(SystemMessageId.IF_YOU_TRY_TO_CHAT_BEFORE_THE_PROHIBITION_IS_REMOVED_THE_PROHIBITION_TIME_WILL_INCREASE_EVEN_FURTHER_S1_SEC_OF_PROHIBITION_IS_LEFT);
			return;
		}
		if (Config.General.JAIL_DISABLE_CHAT && activeChar.isJailed() && !activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
		{
			activeChar.sendPacket(SystemMessageId.CHATTING_IS_CURRENTLY_PROHIBITED);
			return;
		}
		if (activeChar.getLevel() < 20)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.TRADE_CHAT_CANNOT_BE_USED_BY_CHARACTERS_LV_S1_OR_LOWER);
			sm.Params.addInt(20);
			activeChar.sendPacket(sm);
			return;
		}

		if (shareLocation)
		{
			if (activeChar.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) < Config.General.SHARING_LOCATION_COST)
			{
				activeChar.sendPacket(SystemMessageId.THERE_ARE_NOT_ENOUGH_L_COINS);
				return;
			}

			if (activeChar.getMovieHolder() != null || activeChar.isFishing() || activeChar.isInInstance() || activeChar.isOnEvent() || activeChar.isInOlympiadMode() || activeChar.inObserverMode() || activeChar.isInTraingCamp() || activeChar.isInTimedHuntingZone() || activeChar.isInsideZone(ZoneId.SIEGE))
			{
				activeChar.sendPacket(SystemMessageId.LOCATION_CANNOT_BE_SHARED_SINCE_THE_CONDITIONS_ARE_NOT_MET);
				return;
			}

			activeChar.destroyItemByItemId("Shared Location", Inventory.LCOIN_ID, Config.General.SHARING_LOCATION_COST, activeChar, true);
		}

		CreatureSayPacket cs = new CreatureSayPacket(activeChar, type, activeChar.getName(), text, shareLocation);
		if (Config.General.DEFAULT_TRADE_CHAT.equalsIgnoreCase("on") || (Config.General.DEFAULT_TRADE_CHAT.equalsIgnoreCase("gm") && activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS)))
		{
			int region = MapRegionData.Instance.GetMapRegionLocationId(activeChar);
			foreach (Player player in World.getInstance().getPlayers())
			{
				if (region == MapRegionData.Instance.GetMapRegionLocationId(player) && !BlockList.isBlocked(player, activeChar) && player.getInstanceId() == activeChar.getInstanceId())
				{
					if (Config.FactionSystem.FACTION_SYSTEM_ENABLED)
					{
						if (Config.FactionSystem.FACTION_SPECIFIC_CHAT)
						{
							if ((activeChar.isGood() && player.isGood()) || (activeChar.isEvil() && player.isEvil()))
							{
								player.sendPacket(cs);
							}
						}
						else
						{
							player.sendPacket(cs);
						}
					}
					else
					{
						player.sendPacket(cs);
					}
				}
			}
		}
		else if (Config.General.DEFAULT_TRADE_CHAT.equalsIgnoreCase("global"))
		{
			// TODO: flood protection
			// if (!activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS) && !activeChar.getClient().getFloodProtectors().canUseGlobalChat())
			// {
			// 	activeChar.sendMessage("Do not spam trade channel.");
			// 	return;
			// }

			foreach (Player player in World.getInstance().getPlayers())
			{
				if (!BlockList.isBlocked(player, activeChar))
				{
					if (Config.FactionSystem.FACTION_SYSTEM_ENABLED)
					{
						if (Config.FactionSystem.FACTION_SPECIFIC_CHAT)
						{
							if ((activeChar.isGood() && player.isGood()) || (activeChar.isEvil() && player.isEvil()))
							{
								player.sendPacket(cs);
							}
						}
						else
						{
							player.sendPacket(cs);
						}
					}
					else
					{
						player.sendPacket(cs);
					}
				}
			}
		}
	}

	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}