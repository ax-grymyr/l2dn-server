using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Matching;

public class PartyMatchingRoom: MatchingRoom
{
	public PartyMatchingRoom(String title, int loot, int minLevel, int maxLevel, int maxmem, Player leader): base(title,
		loot, minLevel, maxLevel, maxmem, leader)
	{
	}

	protected override void onRoomCreation(Player player)
	{
		player.broadcastUserInfo(UserInfoType.CLAN);
		player.sendPacket(new ListPartyWaiting(PartyMatchingRoomLevelType.ALL, -1, 1, player.getLevel()));
		player.sendPacket(SystemMessageId.YOU_HAVE_CREATED_A_PARTY_ROOM);
	}

	protected override void notifyInvalidCondition(Player player)
	{
		player.sendPacket(SystemMessageId.YOU_DON_T_MEET_THE_REQUIREMENTS_TO_ENTER_A_PARTY_ROOM);
	}

	protected override void notifyNewMember(Player player)
	{
		// Update other players
		foreach (Player member in getMembers())
		{
			if (member != player)
			{
				member.sendPacket(new ExPartyRoomMember(member, this));
			}
		}

		// Send SystemMessage to other players
		SystemMessage sm = new SystemMessage(SystemMessageId.C1_HAS_ENTERED_THE_PARTY_ROOM);
		sm.addPcName(player);
		foreach (Player member in getMembers())
		{
			if (member != player)
			{
				member.sendPacket(sm);
			}
		}

		// Update new player
		player.sendPacket(new PartyRoomInfo(this));
		player.sendPacket(new ExPartyRoomMember(player, this));
	}

	protected override void notifyRemovedMember(Player player, bool kicked, bool leaderChanged)
	{
		SystemMessage sm = new SystemMessage(kicked
			? SystemMessageId.C1_HAS_BEEN_KICKED_FROM_THE_PARTY_ROOM
			: SystemMessageId.C1_HAS_LEFT_THE_PARTY_ROOM);
		sm.addPcName(player);

		getMembers().forEach(p =>
		{
			p.sendPacket(new PartyRoomInfo(this));
			p.sendPacket(new ExPartyRoomMember(player, this));
			p.sendPacket(sm);
			p.sendPacket(SystemMessageId.THE_LEADER_OF_THE_PARTY_ROOM_HAS_CHANGED);
		});

		player.sendPacket(new SystemMessage(kicked
			? SystemMessageId.YOU_HAVE_BEEN_OUSTED_FROM_THE_PARTY_ROOM
			: SystemMessageId.YOU_HAVE_EXITED_THE_PARTY_ROOM));
		player.sendPacket(ExClosePartyRoom.STATIC_PACKET);
	}

	public override void disbandRoom()
	{
		getMembers().forEach(p =>
		{
			p.sendPacket(SystemMessageId.THE_PARTY_ROOM_HAS_BEEN_DISBANDED);
			p.sendPacket(ExClosePartyRoom.STATIC_PACKET);
			p.setMatchingRoom(null);
			p.broadcastUserInfo(UserInfoType.CLAN);
			MatchingRoomManager.getInstance().addToWaitingList(p);
		});

		getMembers().clear();

		MatchingRoomManager.getInstance().removeMatchingRoom(this);
	}

	public override MatchingRoomType getRoomType()
	{
		return MatchingRoomType.PARTY;
	}

	public override MatchingMemberType getMemberType(Player player)
	{
		if (isLeader(player))
		{
			return MatchingMemberType.PARTY_LEADER;
		}

		Party leaderParty = getLeader().getParty();
		Party playerParty = player.getParty();
		if ((leaderParty != null) && (playerParty != null) && (playerParty == leaderParty))
		{
			return MatchingMemberType.PARTY_MEMBER;
		}

		return MatchingMemberType.WAITING_PLAYER;
	}
}