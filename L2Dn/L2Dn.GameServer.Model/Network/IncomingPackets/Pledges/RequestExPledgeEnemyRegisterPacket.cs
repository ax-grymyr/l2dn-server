using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pledges;

public struct RequestExPledgeEnemyRegisterPacket: IIncomingPacket<GameSession>
{
    private string _pledgeName;

    public void ReadContent(PacketBitReader reader)
    {
        _pledgeName = reader.ReadSizedString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		Clan? playerClan = player.getClan();
		if (playerClan == null)
			return ValueTask.CompletedTask;

		if (!player.hasClanPrivilege(ClanPrivilege.CL_PLEDGE_WAR))
		{
			connection.Send(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (playerClan.getWarCount() >= 30)
		{
			connection.Send(SystemMessageId.A_DECLARATION_OF_WAR_AGAINST_MORE_THAN_30_CLANS_CAN_T_BE_MADE_AT_THE_SAME_TIME);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		Clan? enemyClan = ClanTable.getInstance().getClanByName(_pledgeName);
		if (enemyClan == null)
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.A_CLAN_WAR_CANNOT_BE_DECLARED_AGAINST_A_CLAN_THAT_DOES_NOT_EXIST));
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (enemyClan == playerClan)
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.FOOL_YOU_CANNOT_DECLARE_WAR_AGAINST_YOUR_OWN_CLAN));
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (playerClan.getAllyId() == enemyClan.getAllyId() && playerClan.getAllyId() != 0)
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.A_DECLARATION_OF_CLAN_WAR_AGAINST_AN_ALLIED_CLAN_CAN_T_BE_MADE));
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (enemyClan.getDissolvingExpiryTime() > DateTime.UtcNow)
		{
			connection.Send(new SystemMessagePacket(SystemMessageId.A_CLAN_WAR_CAN_NOT_BE_DECLARED_AGAINST_A_CLAN_THAT_IS_BEING_DISSOLVED));
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		ClanWar clanWar = playerClan.getWarWith(enemyClan.getId());
		if (clanWar != null)
		{
			if (clanWar.getClanWarState(playerClan) == ClanWarState.WIN)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_CAN_T_DECLARE_A_WAR_BECAUSE_THE_21_DAY_PERIOD_HASN_T_PASSED_AFTER_A_DEFEAT_DECLARATION_WITH_THE_S1_CLAN);
				sm.Params.addString(enemyClan.getName());
				connection.Send(sm);
				connection.Send(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (clanWar.getClanWarState(playerClan) != ClanWarState.BLOOD_DECLARATION || clanWar.getAttackerClanId() == playerClan.getId())
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ALREADY_BEEN_AT_WAR_WITH_THE_S1_CLAN_5_DAYS_MUST_PASS_BEFORE_YOU_CAN_DECLARE_WAR_AGAIN);
				sm.Params.addString(enemyClan.getName());
				connection.Send(sm);
				connection.Send(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (clanWar.getClanWarState(playerClan) == ClanWarState.BLOOD_DECLARATION)
			{
				clanWar.mutualClanWarAccepted(enemyClan, playerClan);
				broadcastClanInfo(playerClan, enemyClan);
				return ValueTask.CompletedTask;
			}
		}

		ClanWar newClanWar = new ClanWar(playerClan, enemyClan);
		ClanTable.getInstance().storeClanWars(newClanWar);

		broadcastClanInfo(playerClan, enemyClan);

		return ValueTask.CompletedTask;
	}

	private static void broadcastClanInfo(Clan playerClan, Clan enemyClan)
	{
		foreach (ClanMember member in playerClan.getMembers())
		{
			if (member != null && member.isOnline())
			{
				member.getPlayer().sendPacket(new ExPledgeEnemyInfoListPacket(playerClan));
				member.getPlayer().broadcastUserInfo();
			}
		}

		foreach (ClanMember member in enemyClan.getMembers())
		{
			if (member != null && member.isOnline())
			{
				member.getPlayer().sendPacket(new ExPledgeEnemyInfoListPacket(enemyClan));
				member.getPlayer().broadcastUserInfo();
			}
		}
	}
}