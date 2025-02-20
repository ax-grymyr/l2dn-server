using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Matching;

public class CommandChannelMatchingRoom: MatchingRoom
{
	public CommandChannelMatchingRoom(string title, PartyDistributionType loot, int minLevel, int maxLevel, int maxmem, Player leader):
		base(title, loot, minLevel, maxLevel, maxmem, leader)
	{
	}

	protected override void onRoomCreation(Player player)
	{
		player.sendPacket(SystemMessageId.THE_COMMAND_CHANNEL_MATCHING_ROOM_WAS_CREATED);
	}

	protected override void notifyInvalidCondition(Player player)
	{
		player.sendPacket(SystemMessageId
			.YOU_CANNOT_ENTER_THE_COMMAND_CHANNEL_MATCHING_ROOM_BECAUSE_YOU_DO_NOT_MEET_THE_REQUIREMENTS);
	}

	protected override void notifyNewMember(Player player)
	{
		// Update other players
		foreach (Player member in getMembers())
		{
			if (member != player)
			{
				member.sendPacket(new ExManageMpccRoomMemberPacket(member, this, ExManagePartyRoomMemberType.ADD_MEMBER));
			}
		}

		// Send SystemMessage to other players
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_ENTERED_THE_COMMAND_CHANNEL_MATCHING_ROOM);
		sm.Params.addPcName(player);
		foreach (Player member in getMembers())
		{
			if (member != player)
			{
				member.sendPacket(sm);
			}
		}

		// Update new player
		player.sendPacket(new ExMPCCRoomInfoPacket(this));
		player.sendPacket(new ExMPCCRoomMemberPacket(player, this));
	}

	protected override void notifyRemovedMember(Player player, bool kicked, bool leaderChanged)
	{
		getMembers().ForEach(p =>
		{
			p.sendPacket(new ExMPCCRoomInfoPacket(this));
			p.sendPacket(new ExMPCCRoomMemberPacket(player, this));
		});

		player.sendPacket(new SystemMessagePacket(kicked
			? SystemMessageId.YOU_WERE_EXPELLED_FROM_THE_COMMAND_CHANNEL_MATCHING_ROOM
			: SystemMessageId.YOU_EXITED_FROM_THE_COMMAND_CHANNEL_MATCHING_ROOM));
	}

	public override void disbandRoom()
	{
		getMembers().ForEach(p =>
		{
			p.sendPacket(SystemMessageId.THE_COMMAND_CHANNEL_MATCHING_ROOM_WAS_CANCELLED);
			p.sendPacket(ExDissmissMPCCRoomPacket.STATIC_PACKET);
			p.setMatchingRoom(null);
			p.broadcastUserInfo(UserInfoType.CLAN);
			MatchingRoomManager.getInstance().addToWaitingList(p);
		});

		getMembers().clear();

		MatchingRoomManager.getInstance().removeMatchingRoom(this);
	}

	public override MatchingRoomType getRoomType()
	{
		return MatchingRoomType.COMMAND_CHANNEL;
	}

	public override MatchingMemberType getMemberType(Player player)
	{
		if (isLeader(player))
		{
			return MatchingMemberType.COMMAND_CHANNEL_LEADER;
		}

		Party playerParty = player.getParty();
		if (playerParty == null)
		{
			return MatchingMemberType.WAITING_PLAYER_NO_PARTY;
		}

		Party leaderParty = getLeader().getParty();
		if (leaderParty != null)
		{
			CommandChannel cc = leaderParty.getCommandChannel();
			if (leaderParty == playerParty || (cc != null && cc.getParties().Contains(playerParty)))
			{
				return MatchingMemberType.COMMAND_CHANNEL_PARTY_MEMBER;
			}
		}

		return MatchingMemberType.WAITING_PARTY;
	}
}