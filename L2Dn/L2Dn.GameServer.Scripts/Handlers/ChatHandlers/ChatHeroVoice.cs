using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ChatHandlers;

/**
 * Hero chat handler.
 * @author durgus
 */
public class ChatHeroVoice: IChatHandler
{
	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.HERO_VOICE,
	};
	
	public void handleChat(ChatType type, Player activeChar, string target, string text, bool shareLocation)
	{
		if (!activeChar.isHero() && !activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
		{
			activeChar.sendPacket(SystemMessageId.ONLY_HEROES_CAN_ENTER_THE_HERO_CHANNEL);
			return;
		}
		
		if (activeChar.isChatBanned() && Config.BAN_CHAT_CHANNELS.Contains(type))
		{
			activeChar.sendPacket(SystemMessageId.IF_YOU_TRY_TO_CHAT_BEFORE_THE_PROHIBITION_IS_REMOVED_THE_PROHIBITION_TIME_WILL_INCREASE_EVEN_FURTHER_S1_SEC_OF_PROHIBITION_IS_LEFT);
			return;
		}
		if (Config.JAIL_DISABLE_CHAT && activeChar.isJailed() && !activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
		{
			activeChar.sendPacket(SystemMessageId.CHATTING_IS_CURRENTLY_PROHIBITED);
			return;
		}

		// TODO: flood protection
		// if (!activeChar.getClient().getFloodProtectors().canUseHeroVoice())
		// {
		// 	activeChar.sendMessage("Action failed. Heroes are only able to speak in the global channel once every 10 seconds.");
		// 	return;
		// }
		
		CreatureSayPacket cs = new CreatureSayPacket(activeChar, type, activeChar.getName(), text, shareLocation);
		foreach (Player player in World.getInstance().getPlayers())
		{
			if (player != null && !BlockList.isBlocked(player, activeChar))
			{
				if (Config.FACTION_SYSTEM_ENABLED)
				{
					if (Config.FACTION_SPECIFIC_CHAT)
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
	
	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}