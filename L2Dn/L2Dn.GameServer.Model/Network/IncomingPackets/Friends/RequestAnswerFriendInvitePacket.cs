using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Friends;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Friends;

public struct RequestAnswerFriendInvitePacket: IIncomingPacket<GameSession>
{
    private int _response;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadByte();
        _response = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		Player? requestor = player.getActiveRequester();
		if (requestor == null)
			return ValueTask.CompletedTask;

		if (player == requestor)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_ADD_YOURSELF_TO_YOUR_OWN_FRIEND_LIST);
			return ValueTask.CompletedTask;
		}

		if (player.getFriendList().Contains(requestor.ObjectId) //
			|| requestor.getFriendList().Contains(player.ObjectId))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_ALREADY_ON_YOUR_FRIEND_LIST);
			sm.Params.addString(player.getName());
			requestor.sendPacket(sm);
			return ValueTask.CompletedTask;
		}

		if (_response == 1)
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.CharacterFriends.Add(new CharacterFriend()
				{
					CharacterId = requestor.ObjectId,
					FriendId = player.ObjectId
				});

				ctx.CharacterFriends.Add(new CharacterFriend()
				{
					FriendId = requestor.ObjectId,
					CharacterId = player.ObjectId
				});

				ctx.SaveChanges();

				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.THAT_PERSON_HAS_BEEN_SUCCESSFULLY_ADDED_TO_YOUR_FRIEND_LIST);
				requestor.sendPacket(msg);

				// Player added to your friend list
				msg = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_ADDED_TO_YOUR_FRIEND_LIST);
				msg.Params.addString(player.getName());
				requestor.sendPacket(msg);
				requestor.getFriendList().add(player.ObjectId);

				// has joined as friend.
				msg = new SystemMessagePacket(SystemMessageId.S1_HAS_BEEN_ADDED_TO_YOUR_FRIEND_LIST_2);
				msg.Params.addString(requestor.getName());
				player.sendPacket(msg);
				player.getFriendList().add(requestor.ObjectId);

				// Send notifications for both player in order to show them online
				player.sendPacket(new FriendAddRequestResultPacket(requestor, 1));
				requestor.sendPacket(new FriendAddRequestResultPacket(player, 1));
			}
			catch (Exception e)
			{
				PacketLogger.Instance.Warn("Could not add friend objectid: " + e);
			}
		}
		else
		{
			requestor.sendPacket(SystemMessageId.YOU_HAVE_FAILED_TO_ADD_A_FRIEND_TO_YOUR_FRIENDS_LIST);
		}

		player.setActiveRequester(null);
		requestor.onTransactionResponse();
		return ValueTask.CompletedTask;
    }
}