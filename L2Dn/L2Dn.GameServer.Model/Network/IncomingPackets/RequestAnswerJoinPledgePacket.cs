using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAnswerJoinPledgePacket: IIncomingPacket<GameSession>
{
    private int _answer;

    public void ReadContent(PacketBitReader reader)
    {
        _answer = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		Player requestor = player.getRequest().getPartner();
		if (requestor == null)
			return ValueTask.CompletedTask;

		if (_answer == 0)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_DIDN_T_RESPOND_TO_S1_S_INVITATION_JOINING_HAS_BEEN_CANCELLED);
			sm.Params.addString(requestor.getName());
			player.sendPacket(sm);
			sm = new SystemMessagePacket(SystemMessageId.S1_DID_NOT_RESPOND_INVITATION_TO_THE_CLAN_HAS_BEEN_CANCELLED);
			sm.Params.addString(player.getName());
			requestor.sendPacket(sm);
		}
		else
		{
			if (!(requestor.getRequest().getRequestPacket() is RequestJoinPledgePacket ||
			      requestor.getRequest().getRequestPacket() is RequestClanAskJoinByNamePacket))
			{
				return ValueTask.CompletedTask; // hax
			}

			int pledgeType;
			if (requestor.getRequest().getRequestPacket() is RequestJoinPledgePacket)
			{
				pledgeType = ((RequestJoinPledgePacket)requestor.getRequest().getRequestPacket()).getPledgeType();
			}
			else
			{
				pledgeType = ((RequestClanAskJoinByNamePacket)requestor.getRequest().getRequestPacket()).getPledgeType();
			}

			Clan? clan = requestor.getClan();

			// we must double-check this cause during response time conditions can be changed,
			// i.e. another player could join clan
			if (clan.checkClanJoinCondition(requestor, player, pledgeType))
			{
				if (player.getClan() != null)
				{
					return ValueTask.CompletedTask;
				}

				player.sendPacket(new JoinPledgePacket(requestor.getClanId().Value));
				player.setPledgeType(pledgeType);
				if (pledgeType == Clan.SUBUNIT_ACADEMY)
				{
					player.setPowerGrade(9); // academy
					player.setLvlJoinedAcademy(player.getLevel());
				}
				else
				{
					player.setPowerGrade(5); // new member starts at 5, not confirmed
				}

				clan.addClanMember(player);
				player.setClanPrivileges(player.getClan().getRankPrivs(player.getPowerGrade()));
				player.sendPacket(SystemMessageId.ENTERED_THE_CLAN);

				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_JOINED_THE_CLAN);
				sm.Params.addString(player.getName());
				clan.broadcastToOnlineMembers(sm);

				if (clan.getCastleId() > 0)
				{
					Castle castle = CastleManager.getInstance().getCastleByOwner(clan);
					if (castle != null)
					{
						castle.giveResidentialSkills(player);
					}
				}
				if (clan.getFortId() > 0)
				{
					Fort fort = FortManager.getInstance().getFortByOwner(clan);
					if (fort != null)
					{
						fort.giveResidentialSkills(player);
					}
				}
				player.sendSkillList();

				clan.broadcastToOtherOnlineMembers(new PledgeShowMemberListAddPacket(player), player);
				clan.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(clan));
				clan.broadcastToOnlineMembers(new ExPledgeCountPacket(clan));

				// this activates the clan tab on the new member
				PledgeShowMemberListAllPacket.sendAllTo(player);
				player.setClanJoinExpiryTime(null);
				player.broadcastUserInfo();
			}
		}

		player.getRequest().onRequestResponse();
		return ValueTask.CompletedTask;
    }
}