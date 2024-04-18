using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Model;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct PledgeShowMemberListUpdatePacket: IOutgoingPacket
{
    private readonly int _pledgeType;
    private readonly string _name;
    private readonly int _level;
    private readonly CharacterClass _classId;
    private readonly int _objectId;
    private readonly Race _race;
    private readonly byte _onlineStatus;
    private readonly Sex _sex;
    private readonly bool _hasSponsor;

    public PledgeShowMemberListUpdatePacket(Player player)
        : this(player.getClan().getClanMember(player.getObjectId()))
    {
    }

    public PledgeShowMemberListUpdatePacket(ClanMember member)
    {
        _name = member.getName();
        _level = member.getLevel();
        _classId = member.getClassId();
        _objectId = member.getObjectId();
        _pledgeType = member.getPledgeType();
        _race = member.getRace();
        _sex = member.getSex();
        _onlineStatus = (byte)member.getOnlineStatus();
        _hasSponsor = _pledgeType == Clan.SUBUNIT_ACADEMY && member.getSponsor() != 0;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SHOW_MEMBER_LIST_UPDATE);

        writer.WriteString(_name);
        writer.WriteInt32(_level);
        writer.WriteInt32((int)_classId);
        writer.WriteInt32(_sex == Sex.Female);
        writer.WriteInt32((int)_race);
        if (_onlineStatus > 0)
        {
            writer.WriteInt32(_objectId);
            writer.WriteInt32(_pledgeType);
        }
        else
        {
            // when going offline send as 0
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }

        writer.WriteInt32(_hasSponsor);
        writer.WriteByte(_onlineStatus);
    }
}