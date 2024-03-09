using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.ChatHandlers;

/**
 * Petition chat handler.
 * @author durgus
 */
public class ChatPetition: IChatHandler
{
	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.PETITION_PLAYER,
		ChatType.PETITION_GM,
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
		if (!PetitionManager.getInstance().isPlayerInConsultation(activeChar))
		{
			activeChar.sendPacket(SystemMessageId.YOU_ARE_NOT_IN_THE_GLOBAL_SUPPORT_CHAT);
			return;
		}
		PetitionManager.getInstance().sendActivePetitionMessage(activeChar, text);
	}
	
	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}