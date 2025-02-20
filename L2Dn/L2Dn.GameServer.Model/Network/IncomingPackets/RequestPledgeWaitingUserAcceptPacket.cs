using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeWaitingUserAcceptPacket: IIncomingPacket<GameSession>
{
    private bool _acceptRequest;
    private int _playerId;

    public void ReadContent(PacketBitReader reader)
    {
        _acceptRequest = reader.ReadInt32() == 1;
        _playerId = reader.ReadInt32();
        reader.ReadInt32(); // Clan Id.
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Clan? clan = player.getClan();
		if (clan == null)
			return ValueTask.CompletedTask;

		int clanId = clan.getId();
		if (_acceptRequest)
		{
			Player? target = World.getInstance().getPlayer(_playerId);
			if (target != null)
			{
				DateTime currentTime = DateTime.UtcNow;
				if (target.getClan() == null && target.getClanJoinExpiryTime() < currentTime)
				{
					target.sendPacket(new JoinPledgePacket(clan.getId()));

					// player.setPowerGrade(9); // academy
					target.setPowerGrade(5); // New member starts at 5, not confirmed.
					clan.addClanMember(target);
					target.setClanPrivileges(clan.getRankPrivs(target.getPowerGrade()));
					target.sendPacket(SystemMessageId.ENTERED_THE_CLAN);

					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_JOINED_THE_CLAN);
					sm.Params.addString(target.getName());
					clan.broadcastToOnlineMembers(sm);

					if (clan.getCastleId() > 0)
					{
						Castle? castle = CastleManager.getInstance().getCastleByOwner(clan);
						if (castle != null)
						{
							castle.giveResidentialSkills(target);
						}
					}
					if (clan.getFortId() > 0)
					{
						Fort? fort = FortManager.getInstance().getFortByOwner(clan);
						if (fort != null)
						{
							fort.giveResidentialSkills(target);
						}
					}
					target.sendSkillList();

					clan.broadcastToOtherOnlineMembers(new PledgeShowMemberListAddPacket(target), target);
					clan.broadcastToOnlineMembers(new PledgeShowInfoUpdatePacket(clan));
					clan.broadcastToOnlineMembers(new ExPledgeCountPacket(clan));

					// This activates the clan tab on the new member.
					PledgeShowMemberListAllPacket.sendAllTo(target);
					target.setClanJoinExpiryTime(null);
					player.setClanJoinTime(currentTime);
					target.broadcastUserInfo();

					ClanEntryManager.getInstance().removePlayerApplication(clanId, _playerId);
				}
				else if (target.getClanJoinExpiryTime() > currentTime)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_WILL_BE_ABLE_TO_JOIN_YOUR_CLAN_IN_24_H_AFTER_LEAVING_THE_PREVIOUS_ONE);
					sm.Params.addString(target.getName());
					player.sendPacket(sm);
				}
			}
		}
		else
		{
			ClanEntryManager.getInstance().removePlayerApplication(clanId, _playerId);
		}

        return ValueTask.CompletedTask;
    }
}