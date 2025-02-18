using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestStartPledgeWarPacket: IIncomingPacket<GameSession>
{
    private string _pledgeName;

    public void ReadContent(PacketBitReader reader)
    {
        _pledgeName = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		Clan? clanDeclaringWar = player.getClan();
		if (clanDeclaringWar == null)
			return ValueTask.CompletedTask;

		if (clanDeclaringWar.getLevel() < 3 || clanDeclaringWar.getMembersCount() < Config.ALT_CLAN_MEMBERS_FOR_WAR)
		{
			connection.Send(SystemMessageId.A_CLAN_WAR_CAN_ONLY_BE_DECLARED_IF_THE_CLAN_IS_LV_3_OR_HIGHER_AND_THE_NUMBER_OF_CLAN_MEMBERS_IS_15_OR_GREATER);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (!player.hasClanPrivilege(ClanPrivilege.CL_PLEDGE_WAR))
		{
			connection.Send(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (clanDeclaringWar.getWarCount() >= 30)
		{
			connection.Send(SystemMessageId.A_DECLARATION_OF_WAR_AGAINST_MORE_THAN_30_CLANS_CAN_T_BE_MADE_AT_THE_SAME_TIME);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		Clan? clanDeclaredWar = ClanTable.getInstance().getClanByName(_pledgeName);
		if (clanDeclaredWar == null)
		{
			connection.Send(SystemMessageId.A_CLAN_WAR_CANNOT_BE_DECLARED_AGAINST_A_CLAN_THAT_DOES_NOT_EXIST);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (clanDeclaredWar == clanDeclaringWar)
		{
			connection.Send(SystemMessageId.FOOL_YOU_CANNOT_DECLARE_WAR_AGAINST_YOUR_OWN_CLAN);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (clanDeclaringWar.getAllyId() == clanDeclaredWar.getAllyId() && clanDeclaringWar.getAllyId() != 0)
		{
			connection.Send(SystemMessageId.A_DECLARATION_OF_CLAN_WAR_AGAINST_AN_ALLIED_CLAN_CAN_T_BE_MADE);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (clanDeclaredWar.getLevel() < 3 || clanDeclaredWar.getMembersCount() < Config.ALT_CLAN_MEMBERS_FOR_WAR)
		{
			connection.Send(SystemMessageId.A_CLAN_WAR_CAN_ONLY_BE_DECLARED_IF_THE_CLAN_IS_LV_3_OR_HIGHER_AND_THE_NUMBER_OF_CLAN_MEMBERS_IS_15_OR_GREATER);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (clanDeclaredWar.getDissolvingExpiryTime() > DateTime.UtcNow)
		{
			connection.Send(SystemMessageId.A_CLAN_WAR_CAN_NOT_BE_DECLARED_AGAINST_A_CLAN_THAT_IS_BEING_DISSOLVED);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		ClanWar clanWar = clanDeclaringWar.getWarWith(clanDeclaredWar.getId());
		if (clanWar != null)
		{
			if (clanWar.getClanWarState(clanDeclaringWar) == ClanWarState.WIN)
			{
				SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOU_CAN_T_DECLARE_A_WAR_BECAUSE_THE_21_DAY_PERIOD_HASN_T_PASSED_AFTER_A_DEFEAT_DECLARATION_WITH_THE_S1_CLAN);
				sm.Params.addString(clanDeclaredWar.getName());
				connection.Send(sm);
				connection.Send(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (clanWar.getState() == ClanWarState.MUTUAL)
			{
				connection.Send("You have already been at war with " + clanDeclaredWar.getName() + ".");
				connection.Send(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}

			if (clanWar.getState() == ClanWarState.BLOOD_DECLARATION)
			{
				clanWar.mutualClanWarAccepted(clanDeclaredWar, clanDeclaringWar);
				ClanTable.getInstance().storeClanWars(clanWar);
				foreach (ClanMember member in clanDeclaringWar.getMembers())
				{
					if (member != null && member.isOnline())
					{
						member.getPlayer().broadcastUserInfo(UserInfoType.CLAN);
					}
				}

				foreach (ClanMember member in clanDeclaredWar.getMembers())
				{
					if (member != null && member.isOnline())
					{
						member.getPlayer().broadcastUserInfo(UserInfoType.CLAN);
					}
				}

				connection.Send(new PledgeReceiveWarListPacket(player.getClan(), 0));
				return ValueTask.CompletedTask;
			}
		}

		ClanWar newClanWar = new ClanWar(clanDeclaringWar, clanDeclaredWar);
		ClanTable.getInstance().storeClanWars(newClanWar);

		foreach (ClanMember member in clanDeclaringWar.getMembers())
		{
			if (member != null && member.isOnline())
			{
				member.getPlayer().broadcastUserInfo(UserInfoType.CLAN);
			}
		}

		foreach (ClanMember member in clanDeclaredWar.getMembers())
		{
			if (member != null && member.isOnline())
			{
				member.getPlayer().broadcastUserInfo(UserInfoType.CLAN);
			}
		}

		connection.Send(new PledgeReceiveWarListPacket(player.getClan(), 0));
		return ValueTask.CompletedTask;
    }
}