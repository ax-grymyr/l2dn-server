using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestStopPledgeWarPacket: IIncomingPacket<GameSession>
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

		Clan? playerClan = player.getClan();
		if (playerClan == null)
			return ValueTask.CompletedTask;

		Clan? clan = ClanTable.getInstance().getClanByName(_pledgeName);
		if (clan == null)
		{
			connection.Send("No such clan.");
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		if (!playerClan.isAtWarWith(clan.getId()))
		{
			connection.Send("You aren't at war with this clan.");
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		// Check if player who does the request has the correct rights to do it
		if (!player.hasClanPrivilege(ClanPrivilege.CL_PLEDGE_WAR))
		{
			connection.Send(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
			return ValueTask.CompletedTask;
		}

		// Check if clan has enough reputation to end the war (500).
		if (playerClan.getReputationScore() <= 500)
		{
			connection.Send(SystemMessageId.THE_CLAN_REPUTATION_IS_TOO_LOW);
			connection.Send(ActionFailedPacket.STATIC_PACKET);
			return ValueTask.CompletedTask;
		}

		foreach (ClanMember member in playerClan.getMembers())
		{
			if (member == null || member.getPlayer() == null)
			{
				continue;
			}
			if (AttackStanceTaskManager.getInstance().hasAttackStanceTask(member.getPlayer()))
			{
				connection.Send(SystemMessageId.THE_CLAN_WAR_CANNOT_BE_STOPPED_BECAUSE_SOMEONE_FROM_YOUR_CLAN_IS_STILL_ENGAGED_IN_BATTLE);
				connection.Send(ActionFailedPacket.STATIC_PACKET);
				return ValueTask.CompletedTask;
			}
		}

		// Reduce reputation.
		playerClan.takeReputationScore(500);
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_CLAN_LOST_500_REPUTATION_POINTS_FOR_WITHDRAWING_FROM_THE_CLAN_WAR);
        playerClan.broadcastToOnlineMembers(sm);

		ClanTable.getInstance().deleteClanWars(playerClan.getId(), clan.getId());
		foreach (Player member in playerClan.getOnlineMembers(0))
		{
			member.broadcastUserInfo();
		}

		foreach (Player member in clan.getOnlineMembers(0))
		{
			member.broadcastUserInfo();
		}

		connection.Send(new PledgeReceiveWarListPacket(playerClan, 0));
		return ValueTask.CompletedTask;
    }
}