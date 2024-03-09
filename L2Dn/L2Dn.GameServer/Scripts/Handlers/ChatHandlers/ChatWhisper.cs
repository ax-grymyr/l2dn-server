using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.ChatHandlers;

/**
 * Tell chat handler.
 * @author durgus
 */
public class ChatWhisper: IChatHandler
{
	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.WHISPER
	};
	
	public void handleChat(ChatType type, Player activeChar, String target, String text, bool shareLocation)
	{
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
		
		// Return if no target is set
		if (target == null)
		{
			return;
		}
		
		if (Config.FAKE_PLAYERS_ENABLED && (FakePlayerData.getInstance().getProperName(target) != null))
		{
			if (FakePlayerData.getInstance().isTalkable(target))
			{
				if (Config.FAKE_PLAYER_CHAT)
				{
					String name = FakePlayerData.getInstance().getProperName(target);
					activeChar.sendPacket(new CreatureSayPacket(activeChar, null, "=>" + name, type, text));
					FakePlayerChatManager.getInstance().manageChat(activeChar, name, text);
				}
				else
				{
					activeChar.sendPacket(SystemMessageId.THAT_PERSON_IS_IN_MESSAGE_REFUSAL_MODE);
				}
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
			}
			return;
		}
		
		Player receiver = World.getInstance().getPlayer(target);
		if ((receiver != null) && !receiver.isSilenceMode(activeChar.getObjectId()))
		{
			if (Config.JAIL_DISABLE_CHAT && receiver.isJailed() && !activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
			{
				activeChar.sendMessage("Player is in jail.");
				return;
			}
			if (receiver.isChatBanned())
			{
				activeChar.sendPacket(SystemMessageId.THAT_PERSON_IS_IN_MESSAGE_REFUSAL_MODE);
				return;
			}
			if ((receiver.getClient() == null) || receiver.getClient()?.IsDetached == true)
			{
				activeChar.sendMessage("Player is in offline mode.");
				return;
			}
			if (Config.FACTION_SYSTEM_ENABLED && Config.FACTION_SPECIFIC_CHAT && ((activeChar.isGood() && receiver.isEvil()) || (activeChar.isEvil() && receiver.isGood())))
			{
				activeChar.sendMessage("Player belongs to the opposing faction.");
				return;
			}
			if ((activeChar.getLevel() < Config.MINIMUM_CHAT_LEVEL) && !activeChar.getWhisperers().Contains(receiver.getObjectId()) && !activeChar.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
			{
				var sm = new SystemMessagePacket(SystemMessageId.CHARACTERS_LV_S1_OR_LOWER_CAN_RESPOND_TO_A_WHISPER_BUT_CANNOT_INITIATE_IT);
				sm.Params.addInt(Config.MINIMUM_CHAT_LEVEL);
				activeChar.sendPacket(sm);
				return;
			}
			if (!BlockList.isBlocked(receiver, activeChar))
			{
				// Allow reciever to send PMs to this char, which is in silence mode.
				if (Config.SILENCE_MODE_EXCLUDE && activeChar.isSilenceMode())
				{
					activeChar.addSilenceModeExcluded(receiver.getObjectId());
				}
				
				receiver.getWhisperers().add(activeChar.getObjectId());
				receiver.sendPacket(new CreatureSayPacket(activeChar, receiver, activeChar.getName(), type, text));
				activeChar.sendPacket(new CreatureSayPacket(activeChar, receiver, "=>" + receiver.getName(), type, text));
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.THAT_PERSON_IS_IN_MESSAGE_REFUSAL_MODE);
			}
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.THAT_PLAYER_IS_NOT_ONLINE);
		}
	}
	
	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}