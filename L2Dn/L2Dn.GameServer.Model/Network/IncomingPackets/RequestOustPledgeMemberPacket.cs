using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestOustPledgeMemberPacket: IIncomingPacket<GameSession>
{
    private string _target;

    public void ReadContent(PacketBitReader reader)
    {
        _target = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_A_CLAN_MEMBER_2);
            return ValueTask.CompletedTask;
        }

        if (!player.hasClanPrivilege(ClanPrivilege.CL_DISMISS))
        {
            player.sendPacket(SystemMessageId.YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT);
            return ValueTask.CompletedTask;
        }

        if (string.Equals(player.getName(), _target, StringComparison.InvariantCultureIgnoreCase))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_DISMISS_YOURSELF);
            return ValueTask.CompletedTask;
        }

        ClanMember? member = clan.getClanMember(_target);
        if (member == null)
        {
            PacketLogger.Instance.Warn("Target (" + _target + ") is not member of the clan");
            return ValueTask.CompletedTask;
        }

        Player? memberPlayer = member.getPlayer();
        if (member.isOnline() && memberPlayer != null && memberPlayer.isInCombat())
        {
            player.sendPacket(SystemMessageId.A_CLAN_MEMBER_MAY_NOT_BE_DISMISSED_DURING_COMBAT);
            return ValueTask.CompletedTask;
        }

        // this also updates the database
        clan.removeClanMember(member.getObjectId(), DateTime.UtcNow.AddMinutes(Config.Character.ALT_CLAN_JOIN_MINS));
        clan.setCharPenaltyExpiryTime(DateTime.UtcNow.AddMinutes(Config.Character.ALT_CLAN_JOIN_MINS)); // TODO: check, multiplier was 86400000
        clan.updateClanInDB();

        SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_IS_DISMISSED_FROM_THE_CLAN);
        sm.Params.addString(member.getName());
        clan.broadcastToOnlineMembers(sm);
        player.sendPacket(SystemMessageId.THE_CLAN_MEMBER_IS_DISMISSED);
        player.sendPacket(SystemMessageId.YOU_CANNOT_ACCEPT_A_NEW_CLAN_MEMBER_FOR_24_H_AFTER_DISMISSING_SOMEONE);

        // Remove the Player From the Member list
        clan.broadcastToOnlineMembers(new PledgeShowMemberListDeletePacket(_target));
        clan.broadcastToOnlineMembers(new ExPledgeCountPacket(clan));

        Player? target = member.getPlayer();
        if (member.isOnline() && target != null)
        {
            target.sendPacket(SystemMessageId.YOU_ARE_DISMISSED_FROM_A_CLAN_YOU_CANNOT_JOIN_ANOTHER_FOR_24_H);
        }

        return ValueTask.CompletedTask;
    }
}