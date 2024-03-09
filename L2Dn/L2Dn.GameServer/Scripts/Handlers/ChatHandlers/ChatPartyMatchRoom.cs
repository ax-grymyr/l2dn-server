using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ChatHandlers;

/**
 * Party Match Room chat handler.
 * @author Gnacik
 */
public class ChatPartyMatchRoom: IChatHandler
{
	private static readonly ChatType[] CHAT_TYPES =
	{
		ChatType.PARTYMATCH_ROOM,
	};
	
	public void handleChat(ChatType type, Player activeChar, String target, String text, bool shareLocation)
	{
		MatchingRoom room = activeChar.getMatchingRoom();
		if (room != null)
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
			
			CreatureSayPacket cs = new CreatureSayPacket(activeChar, type, activeChar.getName(), text, shareLocation);
			foreach (Player _member in room.getMembers())
			{
				if (Config.FACTION_SYSTEM_ENABLED)
				{
					if (Config.FACTION_SPECIFIC_CHAT)
					{
						if ((activeChar.isGood() && _member.isGood()) || (activeChar.isEvil() && _member.isEvil()))
						{
							_member.sendPacket(cs);
						}
					}
					else
					{
						_member.sendPacket(cs);
					}
				}
				else
				{
					_member.sendPacket(cs);
				}
			}
		}
	}
	
	public ChatType[] getChatTypeList()
	{
		return CHAT_TYPES;
	}
}