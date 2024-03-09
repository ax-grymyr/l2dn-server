using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.ChatHandlers;

/**
 * Party Room All chat handler.
 * @author durgus
 */
public class ChatPartyRoomAll: IChatHandler
{
	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.PARTYROOM_ALL,
	};
	
	public void handleChat(ChatType type, Player activeChar, String target, String text, bool shareLocation)
	{
		if (activeChar.isInParty() && activeChar.getParty().isInCommandChannel() && activeChar.getParty().isLeader(activeChar))
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
			activeChar.getParty().getCommandChannel().broadcastCreatureSay(new CreatureSayPacket(activeChar, type, activeChar.getName(), text, shareLocation), activeChar);
		}
	}
	
	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}