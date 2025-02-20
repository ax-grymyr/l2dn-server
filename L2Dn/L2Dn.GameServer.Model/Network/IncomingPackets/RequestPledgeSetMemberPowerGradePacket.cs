using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPledgeSetMemberPowerGradePacket: IIncomingPacket<GameSession>
{
    private string _member;
    private int _powerGrade;

    public void ReadContent(PacketBitReader reader)
    {
        _member = reader.ReadString();
        _powerGrade = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Clan? clan = player.getClan();
        if (clan == null)
            return ValueTask.CompletedTask;

        if (!player.hasClanPrivilege(ClanPrivilege.CL_MANAGE_RANKS))
            return ValueTask.CompletedTask;

        ClanMember? member = clan.getClanMember(_member);
        if (member == null)
            return ValueTask.CompletedTask;

        if (member.getObjectId() == clan.getLeaderId())
            return ValueTask.CompletedTask;

        if (member.getPledgeType() == Clan.SUBUNIT_ACADEMY)
        {
            // also checked from client side
            player.sendPacket(SystemMessageId.THAT_PRIVILEGE_CANNOT_BE_GRANTED_TO_A_CLAN_ACADEMY_MEMBER);
            return ValueTask.CompletedTask;
        }

        member.setPowerGrade(_powerGrade);
        clan.broadcastToOnlineMembers(new PledgeShowMemberListUpdatePacket(member));

        SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CLAN_MEMBER_C1_S_PRIVILEGE_LEVEL_HAS_BEEN_CHANGED_TO_S2);
        sm.Params.addString(member.getName()).addInt(_powerGrade);
        clan.broadcastToOnlineMembers(sm);

        // Fixes sometimes not updating when member privileges change.
        clan.broadcastClanStatus();

        return ValueTask.CompletedTask;
    }
}