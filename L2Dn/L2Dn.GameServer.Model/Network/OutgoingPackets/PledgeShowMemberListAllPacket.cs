using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;
using Clan = L2Dn.GameServer.Model.Clans.Clan;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeShowMemberListAllPacket: IOutgoingPacket
{
    private readonly Clan _clan;
    private readonly string _name;
    private readonly string _leaderName;
    private readonly ICollection<ClanMember> _members;
    private readonly int _pledgeId;
    private readonly bool _isSubPledge;

    private PledgeShowMemberListAllPacket(Clan clan, Clan.SubPledge? pledge, bool isSubPledge)
    {
        _clan = clan;
        _pledgeId = pledge?.getId() ?? 0;
        _leaderName = (pledge == null ? clan.getLeaderName() : CharInfoTable.getInstance().getNameById(pledge.getLeaderId())) ?? string.Empty;
        _name = pledge == null ? clan.getName() : pledge.getName();
        _members = _clan.getMembers();
        _isSubPledge = isSubPledge;
    }

    public static void sendAllTo(Player player)
    {
        Clan? clan = player.getClan();
        if (clan != null)
        {
            foreach (Clan.SubPledge subPledge in clan.getAllSubPledges())
            {
                player.sendPacket(new PledgeShowMemberListAllPacket(clan, subPledge, false));
            }

            player.sendPacket(new PledgeShowMemberListAllPacket(clan, null, true));
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SHOW_MEMBER_LIST_ALL);

        writer.WriteInt32(!_isSubPledge);
        writer.WriteInt32(_clan.getId());
        writer.WriteInt32(Config.SERVER_ID);
        writer.WriteInt32(_pledgeId);
        writer.WriteString(_name);
        writer.WriteString(_leaderName);
        writer.WriteInt32(_clan.getCrestId() ?? 0); // crest id .. is used again
        writer.WriteInt32(_clan.getLevel());
        writer.WriteInt32(_clan.getCastleId() ?? 0);
        writer.WriteInt32(0);
        writer.WriteInt32(_clan.getHideoutId());
        writer.WriteInt32(_clan.getFortId() ?? 0);
        writer.WriteInt32(_clan.getRank());
        writer.WriteInt32(_clan.getReputationScore());
        writer.WriteInt32(0); // 0
        writer.WriteInt32(0); // 0
        writer.WriteInt32(_clan.getAllyId() ?? 0);
        writer.WriteString(_clan.getAllyName() ?? string.Empty);
        writer.WriteInt32(_clan.getAllyCrestId() ?? 0);
        writer.WriteInt32(_clan.isAtWar()); // new c3
        writer.WriteInt32(0); // Territory castle ID
        writer.WriteInt32(_clan.getSubPledgeMembersCount(_pledgeId));
        foreach (ClanMember m in _members)
        {
            if (m.getPledgeType() != _pledgeId)
            {
                continue;
            }

            writer.WriteString(m.getName());
            writer.WriteInt32(m.getLevel());
            writer.WriteInt32((int)m.getClassId());

            Player? player = m.getPlayer();
            if (player != null)
            {
                writer.WriteInt32(player.getAppearance().getSex() == Sex.Female); // no visible effect
                writer.WriteInt32((int)player.getRace()); // writer.WriteInt32 (1);
            }
            else
            {
                writer.WriteInt32(1); // no visible effect
                writer.WriteInt32(1); // writer.WriteInt32 (1);
            }

            writer.WriteInt32(m.isOnline() ? m.getObjectId() : 0); // objectId = online 0 = offline
            writer.WriteInt32(m.getSponsor() != 0);
            writer.WriteByte((byte)m.getOnlineStatus());
        }
    }
}