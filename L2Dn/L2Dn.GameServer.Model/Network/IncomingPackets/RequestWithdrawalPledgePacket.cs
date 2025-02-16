using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestWithdrawalPledgePacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (player.getClan() == null)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_A_CLAN_MEMBER_2);
            return ValueTask.CompletedTask;
        }

        if (player.isClanLeader())
        {
            player.sendPacket(SystemMessageId.A_CLAN_LEADER_CANNOT_WITHDRAW_FROM_THEIR_OWN_CLAN);
            return ValueTask.CompletedTask;
        }

        if (player.isInCombat())
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_LEAVE_A_CLAN_WHILE_ENGAGED_IN_COMBAT);
            return ValueTask.CompletedTask;
        }
		
        Clan clan = player.getClan();
        clan.removeClanMember(player.ObjectId, DateTime.UtcNow.AddMinutes(Config.ALT_CLAN_JOIN_MINS));
		
        SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_HAS_LEFT_THE_CLAN);
        sm.Params.addString(player.getName());
        clan.broadcastToOnlineMembers(sm);
		
        // Remove the Player From the Member list
        clan.broadcastToOnlineMembers(new PledgeShowMemberListDeletePacket(player.getName()));
        clan.broadcastToOnlineMembers(new ExPledgeCountPacket(clan));
        player.sendPacket(SystemMessageId.YOU_HAVE_LEFT_THE_CLAN);
        player.sendPacket(SystemMessageId.YOU_CANNOT_JOIN_ANOTHER_CLAN_FOR_24_H_AFTER_LEAVING_THE_PREVIOUS_ONE);
        return ValueTask.CompletedTask;
    }
}