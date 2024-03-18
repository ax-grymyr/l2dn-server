using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestJoinPartyPacket: IIncomingPacket<GameSession>
{
    private string _name;
    private PartyDistributionType _partyDistributionType;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
        _partyDistributionType = (PartyDistributionType)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? requestor = session.Player;
		if (requestor == null)
			return ValueTask.CompletedTask;
		
		if (!Enum.IsDefined(_partyDistributionType))
			return ValueTask.CompletedTask;
		
		ClientSettings clientSettings = requestor.getClientSettings();
		if (clientSettings.getPartyContributionType() != _partyDistributionType)
		{
			requestor.getClientSettings().setPartyContributionType(_partyDistributionType);
		}

		SystemMessagePacket sm;
		if (FakePlayerData.getInstance().isTalkable(_name))
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BEEN_INVITED_TO_THE_PARTY);
			sm.Params.addString(FakePlayerData.getInstance().getProperName(_name));
			requestor.sendPacket(sm);
			if (!requestor.isProcessingRequest())
			{
				ThreadPool.schedule(() => scheduleDeny(requestor), 10000);
				requestor.blockRequest();
			}
			else
			{
				requestor.sendPacket(SystemMessageId.WAITING_FOR_ANOTHER_REPLY);
			}
			
			return ValueTask.CompletedTask;
		}
		
		Player target = World.getInstance().getPlayer(_name);
		if (target == null)
		{
			requestor.sendPacket(SystemMessageId.SELECT_A_PLAYER_YOU_WANT_TO_INVITE_TO_YOUR_PARTY);
			return ValueTask.CompletedTask;
		}
		
		if (target.getClient() == null || target.getClient().IsDetached)
		{
			requestor.sendMessage("Player is in offline mode.");
			return ValueTask.CompletedTask;
		}
		
		if (requestor.isPartyBanned())
		{
			requestor.sendPacket(SystemMessageId.YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_PARTICIPATING_IN_A_PARTY_IS_NOT_ALLOWED);
			requestor.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}
		
		if (target.isPartyBanned())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_AND_CANNOT_JOIN_A_PARTY);
			sm.Params.addString(target.getName());
			requestor.sendPacket(sm);
			return ValueTask.CompletedTask;
		}
		
		if (requestor.isRegisteredOnEvent())
		{
			requestor.sendMessage("You cannot invite to a party while participating in an event.");
			return ValueTask.CompletedTask;
		}
		
		if (target.isInParty())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_A_MEMBER_OF_ANOTHER_PARTY_AND_CANNOT_BE_INVITED);
			sm.Params.addString(target.getName());
			requestor.sendPacket(sm);
			return ValueTask.CompletedTask;
		}
		
		if (BlockList.isBlocked(target, requestor))
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_HAS_ADDED_YOU_TO_THEIR_IGNORE_LIST);
			sm.Params.addString(target.getName());
			requestor.sendPacket(sm);
			return ValueTask.CompletedTask;
		}
		
		if (target == requestor)
		{
			requestor.sendPacket(SystemMessageId.THE_TARGET_CANNOT_BE_INVITED);
			return ValueTask.CompletedTask;
		}

		if (checkInviteByIgnoredSettings(target, requestor))
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_SET_TO_REFUSE_PARTY_REQUESTS_AND_CANNOT_RECEIVE_A_PARTY_REQUEST);
			sm.Params.addPcName(target);
			requestor.sendPacket(sm);

			sm = new SystemMessagePacket(SystemMessageId.PARTY_INVITATION_IS_SET_UP_TO_BE_REJECTED_AT_PREFERENCES_THE_PARTY_INVITATION_OF_C1_IS_AUTOMATICALLY_REJECTED);
			sm.Params.addPcName(requestor);
			target.sendPacket(sm);
			
			return ValueTask.CompletedTask;
		}

		if (target.isCursedWeaponEquipped() || requestor.isCursedWeaponEquipped())
		{
			requestor.sendPacket(SystemMessageId.INVALID_TARGET);
			return ValueTask.CompletedTask;
		}
		
		if (target.isJailed() || requestor.isJailed())
		{
			requestor.sendMessage("You cannot invite a player while is in Jail.");
			return ValueTask.CompletedTask;
		}
		
		if ((target.isInOlympiadMode() || requestor.isInOlympiadMode()) &&
		    (target.isInOlympiadMode() != requestor.isInOlympiadMode() || 
		     target.getOlympiadGameId() != requestor.getOlympiadGameId() || 
		     target.getOlympiadSide() != requestor.getOlympiadSide()))
		{
			requestor.sendPacket(SystemMessageId.A_USER_CURRENTLY_PARTICIPATING_IN_THE_OLYMPIAD_CANNOT_SEND_PARTY_AND_FRIEND_INVITATIONS);
			return ValueTask.CompletedTask;
		}
		
		sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BEEN_INVITED_TO_THE_PARTY);
		sm.Params.addString(target.getName());
		requestor.sendPacket(sm);
		
		if (!requestor.isInParty())
		{
			createNewParty(target, requestor, _partyDistributionType);
		}
		else
		{
			addTargetToParty(target, requestor);
		}

		return ValueTask.CompletedTask;
	}
	
	/**
	 * @param target
	 * @param requestor
	 */
	private void addTargetToParty(Player target, Player requestor)
	{
		Party party = requestor.getParty();
		
		// summary of ppl already in party and ppl that get invitation
		if (!party.isLeader(requestor))
		{
			requestor.sendPacket(SystemMessageId.ONLY_THE_LEADER_CAN_GIVE_OUT_INVITATIONS);
		}
		else if (party.getMemberCount() >= Config.ALT_PARTY_MAX_MEMBERS)
		{
			requestor.sendPacket(SystemMessageId.THE_PARTY_IS_FULL);
		}
		else if (party.getPendingInvitation() && !party.isInvitationRequestExpired())
		{
			requestor.sendPacket(SystemMessageId.WAITING_FOR_ANOTHER_REPLY);
		}
		else if (!target.hasRequest<PartyRequest>())
		{
			PartyRequest request = new PartyRequest(requestor, target, party);
			request.scheduleTimeout(TimeSpan.FromSeconds(30));
			requestor.addRequest(request);
			target.addRequest(request);
			target.sendPacket(new AskJoinPartyPacket(requestor.getName(), party.getDistributionType()));
			party.setPendingInvitation(true);
		}
		else
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
			sm.Params.addString(target.getName());
			requestor.sendPacket(sm);
		}
	}
	
	/**
	 * @param target
	 * @param requestor
	 */
	private static void createNewParty(Player target, Player requestor, PartyDistributionType distributionType)
	{
		if (!target.hasRequest<PartyRequest>())
		{
			Party party = new Party(requestor, distributionType);
			party.setPendingInvitation(true);
			PartyRequest request = new PartyRequest(requestor, target, party);
			request.scheduleTimeout(TimeSpan.FromSeconds(30));
			requestor.addRequest(request);
			target.addRequest(request);
			target.sendPacket(new AskJoinPartyPacket(requestor.getName(), distributionType));
		}
		else
		{
			requestor.sendPacket(SystemMessageId.WAITING_FOR_ANOTHER_REPLY);
		}
	}

	private static bool checkInviteByIgnoredSettings(Player target, Player requestor)
	{
		ClientSettings targetClientSettings = target.getClientSettings();
		bool condition = targetClientSettings.isPartyRequestRestrictedFromOthers();
		bool clanCheck = target.getClan() != null && requestor.getClan() != null &&
		                 target.getClan() == requestor.getClan();
		
		if (condition &&
		    ((!targetClientSettings.isPartyRequestRestrictedFromFriends() &&
		      target.getFriendList().Contains(requestor.getObjectId())) ||
		     (!targetClientSettings.isPartyRequestRestrictedFromClan() && clanCheck)))
		{
			condition = false;
		}

		return condition;
	}

	private static void scheduleDeny(Player player)
	{
		if (player != null)
		{
			if (player.getParty() == null)
			{
				player.sendPacket(SystemMessageId.THE_PARTY_IS_DISBANDED);
			}
			else
			{
				player.sendPacket(SystemMessageId.THE_PLAYER_HAS_DECLINED_TO_JOIN_YOUR_PARTY);
			}
			
			player.onTransactionResponse();
		}
	}
}