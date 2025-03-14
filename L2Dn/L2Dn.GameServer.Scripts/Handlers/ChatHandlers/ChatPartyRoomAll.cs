using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.ChatHandlers;

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

	public void handleChat(ChatType type, Player activeChar, string target, string text, bool shareLocation)
    {
        Party? party = activeChar.getParty();
        CommandChannel? commandChannel = party?.getCommandChannel();
		if (activeChar.isInParty() && party != null && party.isInCommandChannel() && commandChannel != null && party.isLeader(activeChar))
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

            commandChannel.broadcastCreatureSay(new CreatureSayPacket(activeChar, type, activeChar.getName(), text, shareLocation), activeChar);
		}
	}

	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}