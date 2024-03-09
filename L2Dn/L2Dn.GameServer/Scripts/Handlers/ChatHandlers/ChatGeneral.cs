using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ChatHandlers;

/**
 * General Chat Handler.
 * @author durgus
 */
public class ChatGeneral: IChatHandler
{
	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.GENERAL,
	};
	
	public void handleChat(ChatType type, Player activeChar, String paramsValue, String text, bool shareLocation)
	{
		bool vcdUsed = false;
		if (text.startsWith("."))
		{
			StringTokenizer st = new StringTokenizer(text);
			IVoicedCommandHandler vch;
			String command = "";
			String @params = paramsValue;
			if (st.countTokens() > 1)
			{
				command = st.nextToken().Substring(1);
				@params = text.Substring(command.Length + 2);
			}
			else
			{
				command = text.Substring(1);
			}
			vch = VoicedCommandHandler.getInstance().getHandler(command);
			if (vch != null)
			{
				vch.useVoicedCommand(command, activeChar, @params);
				vcdUsed = true;
			}
			else
			{
				vcdUsed = false;
			}
		}
		
		if (!vcdUsed)
		{
			if (activeChar.isChatBanned() && Config.BAN_CHAT_CHANNELS.Contains(type))
			{
				activeChar.sendPacket(SystemMessageId.IF_YOU_TRY_TO_CHAT_BEFORE_THE_PROHIBITION_IS_REMOVED_THE_PROHIBITION_TIME_WILL_INCREASE_EVEN_FURTHER_S1_SEC_OF_PROHIBITION_IS_LEFT);
				return;
			}
			
			if ((activeChar.getLevel() < Config.MINIMUM_CHAT_LEVEL) && !activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.GENERAL_CHAT_CANNOT_BE_USED_BY_CHARACTERS_LV_S1_OR_LOWER);
				sm.Params.addInt(Config.MINIMUM_CHAT_LEVEL);
				activeChar.sendPacket(sm);
				return;
			}
			
			if (shareLocation)
			{
				if (activeChar.getInventory().getInventoryItemCount(Inventory.LCOIN_ID, -1) < Config.SHARING_LOCATION_COST)
				{
					activeChar.sendPacket(SystemMessageId.THERE_ARE_NOT_ENOUGH_L_COINS);
					return;
				}
				
				if ((activeChar.getMovieHolder() != null) || activeChar.isFishing() || activeChar.isInInstance() || activeChar.isOnEvent() || activeChar.isInOlympiadMode() || activeChar.inObserverMode() || activeChar.isInTraingCamp() || activeChar.isInTimedHuntingZone() || activeChar.isInsideZone(ZoneId.SIEGE))
				{
					activeChar.sendPacket(SystemMessageId.LOCATION_CANNOT_BE_SHARED_SINCE_THE_CONDITIONS_ARE_NOT_MET);
					return;
				}
				
				activeChar.destroyItemByItemId("Shared Location", Inventory.LCOIN_ID, Config.SHARING_LOCATION_COST, activeChar, true);
			}
			
			CreatureSayPacket cs = new CreatureSayPacket(activeChar, type, activeChar.getAppearance().getVisibleName(), text, shareLocation);
			CreatureSayPacket csRandom = new CreatureSayPacket(activeChar, type, activeChar.getAppearance().getVisibleName(), ChatRandomizer.randomize(text), shareLocation);
			
			World.getInstance().forEachVisibleObjectInRange<Player>(activeChar, 1250, player =>
			{
				if ((player != null) && !BlockList.isBlocked(player, activeChar))
				{
					if (Config.FACTION_SYSTEM_ENABLED)
					{
						if (Config.FACTION_SPECIFIC_CHAT)
						{
							if ((activeChar.isGood() && player.isEvil()) || (activeChar.isEvil() && player.isGood()))
							{
								player.sendPacket(csRandom);
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
					else
					{
						player.sendPacket(cs);
					}
				}
			});
			
			activeChar.sendPacket(cs);
		}
	}
	
	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}