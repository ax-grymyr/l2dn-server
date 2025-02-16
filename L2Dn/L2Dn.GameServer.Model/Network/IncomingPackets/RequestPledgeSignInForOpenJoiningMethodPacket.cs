using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Clans.Entries;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeSignInForOpenJoiningMethodPacket: IIncomingPacket<GameSession>
{
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        _clanId = reader.ReadInt32();
        reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    PledgeRecruitInfo pledgeRecruitInfo = ClanEntryManager.getInstance().getClanById(_clanId);
	    if (pledgeRecruitInfo == null)
		    return ValueTask.CompletedTask;

	    Clan clan = pledgeRecruitInfo.getClan();
	    if (clan == null || player.getClan() != null)
		    return ValueTask.CompletedTask;

	    if (clan.getCharPenaltyExpiryTime() > DateTime.UtcNow)
	    {
		    player.sendPacket(SystemMessageId.YOU_CANNOT_ACCEPT_A_NEW_CLAN_MEMBER_FOR_24_H_AFTER_DISMISSING_SOMEONE);
		    return ValueTask.CompletedTask;
	    }

	    SystemMessagePacket sm;
	    if (player.getClanJoinExpiryTime() > DateTime.UtcNow)
	    {
		    sm = new SystemMessagePacket(SystemMessageId.C1_WILL_BE_ABLE_TO_JOIN_YOUR_CLAN_IN_S2_MIN_AFTER_LEAVING_THE_PREVIOUS_ONE);
		    sm.Params.addString(player.getName());
		    sm.Params.addInt(Config.ALT_CLAN_JOIN_MINS);
		    player.sendPacket(sm);
		    return ValueTask.CompletedTask;
	    }

	    if (clan.getSubPledgeMembersCount(0) >= clan.getMaxNrOfMembers(0))
	    {
		    sm = new SystemMessagePacket(SystemMessageId.S1_IS_FULL_AND_CANNOT_ACCEPT_ADDITIONAL_CLAN_MEMBERS_AT_THIS_TIME);
		    sm.Params.addString(clan.getName());
		    player.sendPacket(sm);
		    return ValueTask.CompletedTask;
	    }

	    player.sendPacket(new JoinPledgePacket(clan.getId()));

	    // player.setPowerGrade(9); // academy
	    player.setPowerGrade(5); // New member starts at 5, not confirmed.
	    clan.addClanMember(player);
	    player.setClanPrivileges(player.getClan().getRankPrivs(player.getPowerGrade()));
	    player.sendPacket(SystemMessageId.ENTERED_THE_CLAN);

	    sm = new SystemMessagePacket(SystemMessageId.S1_HAS_JOINED_THE_CLAN);
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

	    // This activates the clan tab on the new member.
	    PledgeShowMemberListAllPacket.sendAllTo(player);
	    player.setClanJoinExpiryTime(null);
	    player.setClanJoinTime(DateTime.UtcNow);
	    player.broadcastUserInfo();

	    ClanEntryManager.getInstance().removePlayerApplication(_clanId, player.ObjectId);

	    return ValueTask.CompletedTask;
    }
}