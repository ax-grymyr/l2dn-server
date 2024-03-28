using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestNewInvitePartyInquiryPacket: IIncomingPacket<GameSession>
{
    private int _reqType;
    private ChatType _sayType;

    public void ReadContent(PacketBitReader reader)
    {
        _reqType = reader.ReadByte();
		
        int chatTypeValue = reader.ReadByte();
        _sayType = chatTypeValue switch
        {
            0 => ChatType.GENERAL,
            1 => ChatType.SHOUT,
            3 => ChatType.PARTY,
            4 => ChatType.CLAN,
            8 => ChatType.TRADE,
            _ => default
        };
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		if (player.isChatBanned())
		{
			player.sendPacket(SystemMessageId.YOU_ARE_NOT_ALLOWED_TO_CHAT_WITH_A_CONTACT_WHILE_A_CHATTING_BLOCK_IS_IMPOSED);
			return ValueTask.CompletedTask;
		}
		
		// Ten second delay.
		// TODO: Create another flood protection for this?
		// if (!client.getFloodProtectors().canSendMail())
		// {
		// 	return;
		// }
		
		if (Config.JAIL_DISABLE_CHAT && player.isJailed() && !player.canOverrideCond(PlayerCondOverride.CHAT_CONDITIONS))
		{
			player.sendPacket(SystemMessageId.CHATTING_IS_CURRENTLY_PROHIBITED);
			return ValueTask.CompletedTask;
		}
		
		if (player.isInOlympiadMode())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_CHAT_WHILE_PARTICIPATING_IN_THE_OLYMPIAD);
			return ValueTask.CompletedTask;
		}

		if (_sayType != ChatType.GENERAL && _sayType != ChatType.TRADE && _sayType != ChatType.SHOUT &&
		    _sayType != ChatType.CLAN && _sayType != ChatType.ALLIANCE)
		{
			return ValueTask.CompletedTask;
		}

		switch (_reqType)
		{
			case 0: // Party
			{
				if (player.isInParty())
					return ValueTask.CompletedTask;

				break;
			}
			case 1: // Command Channel
			{
				Party party = player.getParty();
				if (party == null || !party.isLeader(player) || party.getCommandChannel() != null)
					return ValueTask.CompletedTask;

				break;
			}
		}
		
		switch (_sayType)
		{
			case ChatType.SHOUT:
			{
				if (player.inObserverMode())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CHAT_WHILE_IN_THE_SPECTATOR_MODE);
					return ValueTask.CompletedTask;
				}
				
				Broadcast.toAllOnlinePlayers(new ExRequestNewInvitePartyInquiryPacket(player, _reqType, _sayType));
				break;
			}
			
			case ChatType.TRADE:
			{
				if (player.inObserverMode())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CHAT_WHILE_IN_THE_SPECTATOR_MODE);
					return ValueTask.CompletedTask;
				}
				
				Broadcast.toAllOnlinePlayers(new ExRequestNewInvitePartyInquiryPacket(player, _reqType, _sayType));
				break;
			}
			
			case ChatType.GENERAL:
			{
				if (player.inObserverMode())
				{
					player.sendPacket(SystemMessageId.YOU_CANNOT_CHAT_WHILE_IN_THE_SPECTATOR_MODE);
					return ValueTask.CompletedTask;
				}
				
				ExRequestNewInvitePartyInquiryPacket msg = new ExRequestNewInvitePartyInquiryPacket(player, _reqType, _sayType);
				player.sendPacket(msg);
				World.getInstance().forEachVisibleObjectInRange<Player>(player, Config.ALT_PARTY_RANGE, nearby => nearby.sendPacket(msg));
				break;
			}
			
			case ChatType.CLAN:
			{
				Clan clan = player.getClan();
				if (clan == null)
				{
					player.sendPacket(SystemMessageId.YOU_ARE_NOT_IN_A_CLAN);
					return ValueTask.CompletedTask;
				}
				
				clan.broadcastToOnlineMembers(new ExRequestNewInvitePartyInquiryPacket(player, _reqType, _sayType));
				break;
			}
			
			case ChatType.ALLIANCE:
			{
				if (player.getClan() == null || (player.getClan() != null && player.getClan().getAllyId() == 0))
				{
					player.sendPacket(SystemMessageId.YOU_ARE_NOT_IN_AN_ALLIANCE);
					return ValueTask.CompletedTask;
				}
				
				player.getClan().broadcastToOnlineAllyMembers(new ExRequestNewInvitePartyInquiryPacket(player, _reqType, _sayType));
				break;
			}
		}
        
        return ValueTask.CompletedTask;
    }
}