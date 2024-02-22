using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeShowMemberListAddPacket: IOutgoingPacket
{
    private readonly string _name;
    private readonly int _level;
    private readonly CharacterClass _classId;
    private readonly int _isOnline;
    private readonly int _pledgeType;
	
    public PledgeShowMemberListAddPacket(Player player)
    {
        _name = player.getName();
        _level = player.getLevel();
        _classId = player.getClassId();
        _isOnline = player.isOnline() ? player.getObjectId() : 0;
        _pledgeType = player.getPledgeType();
    }
	
    public PledgeShowMemberListAddPacket(ClanMember cm)
    {
        _name = cm.getName();
        _level = cm.getLevel();
        _classId = cm.getClassId();
        _isOnline = (cm.isOnline() ? cm.getObjectId() : 0);
        _pledgeType = cm.getPledgeType();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_SHOW_MEMBER_LIST_ADD);
        
        writer.WriteString(_name);
        writer.WriteInt32(_level);
        writer.WriteInt32((int)_classId);
        writer.WriteInt32(0);
        writer.WriteInt32(1);
        writer.WriteInt32(_isOnline); // 1 = online 0 = offline
        writer.WriteInt32(_pledgeType);
    }
}