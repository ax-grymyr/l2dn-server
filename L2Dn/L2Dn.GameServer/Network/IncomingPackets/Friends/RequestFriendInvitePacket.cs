using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets.Friends;

public struct RequestFriendInvitePacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    SystemMessagePacket sm;
		if (FakePlayerData.getInstance().isTalkable(_name))
		{
			if (!player.isProcessingRequest())
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_VE_REQUESTED_C1_TO_BE_ON_YOUR_FRIENDS_LIST);
				sm.Params.addString(_name);
				player.sendPacket(sm);
				ThreadPool.schedule(() => scheduleDeny(player), 10000);
				player.blockRequest();
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
				sm.Params.addString(_name);
				player.sendPacket(sm);
			}
			
			return ValueTask.CompletedTask;
		}
		
		Player friend = World.getInstance().getPlayer(_name);
		
		// Target is not found in the game.
		if ((friend == null) || !friend.isOnline() || friend.isInvisible())
		{
			player.sendPacket(SystemMessageId.THE_USER_WHO_REQUESTED_TO_BECOME_FRIENDS_IS_NOT_FOUND_IN_THE_GAME);
			return ValueTask.CompletedTask;
		}

		// You cannot add yourself to your own friend list.
		if (friend == player)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ADD_YOURSELF_TO_YOUR_OWN_FRIEND_LIST);
			return ValueTask.CompletedTask;
		}

		// Target is in olympiad.
		if (player.isInOlympiadMode() || friend.isInOlympiadMode())
		{
			player.sendPacket(SystemMessageId.A_USER_CURRENTLY_PARTICIPATING_IN_THE_OLYMPIAD_CANNOT_SEND_PARTY_AND_FRIEND_INVITATIONS);
			return ValueTask.CompletedTask;
		}
		
		// Cannot request friendship in an event.
		if (player.isOnEvent())
		{
			player.sendMessage("You cannot request friendship while participating in an event.");
			return ValueTask.CompletedTask;
		}
		
		// Target blocked active player.
		if (BlockList.isBlocked(friend, player))
		{
			player.sendMessage("You are in target's block list.");
			return ValueTask.CompletedTask;
		}

		// Target is blocked.
		if (BlockList.isBlocked(player, friend))
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_BLOCKED_C1);
			sm.Params.addString(friend.getName());
			player.sendPacket(sm);
		    return ValueTask.CompletedTask;
		}
		
		// Target already in friend list.
		if (player.getFriendList().Contains(friend.getObjectId()))
		{
			player.sendPacket(SystemMessageId.THIS_PLAYER_IS_ALREADY_REGISTERED_ON_YOUR_FRIENDS_LIST);
			return ValueTask.CompletedTask;
		}
		
		// Target is busy.
		if (friend.isProcessingRequest())
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
			sm.Params.addString(_name);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}
		
		// Check, if tatget blocked sends requests in game.
		if (checkInviteByIgnoredSettings(friend, player))
		{
			sm = new SystemMessagePacket(SystemMessageId.PREFERENCES_IS_CONFIGURED_TO_REFUSE_FRIEND_REQUESTS_AND_THE_FRIEND_INVITATION_OF_C1_IS_AUTOMATICALLY_REJECTED);
			sm.Params.addPcName(friend);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}
		// Friend request sent.
		player.onTransactionRequest(friend);
		friend.sendPacket(new FriendAddRequestPacket(player.getName()));
		
		sm = new SystemMessagePacket(SystemMessageId.YOU_VE_REQUESTED_C1_TO_BE_ON_YOUR_FRIENDS_LIST);
		sm.Params.addString(_name);
		player.sendPacket(sm);
		
		return ValueTask.CompletedTask;
	}

    private static bool checkInviteByIgnoredSettings(Player target, Player requestor)
    {
	    ClientSettings targetClientSettings = target.getClientSettings();
	    bool condition = targetClientSettings.isFriendRequestRestrictedFromOthers();
	    if (condition && !targetClientSettings.isFriendRequestRestrictedFromClan() && (target.getClan() != null) &&
	        (requestor.getClan() != null) && (target.getClan() == requestor.getClan()))
	    {
		    return false;
	    }

	    return condition;
    }

    private static void scheduleDeny(Player player)
    {
        if (player != null)
        {
            player.sendPacket(SystemMessageId.YOU_HAVE_FAILED_TO_ADD_A_FRIEND_TO_YOUR_FRIENDS_LIST);
            player.onTransactionResponse();
        }
    }
}