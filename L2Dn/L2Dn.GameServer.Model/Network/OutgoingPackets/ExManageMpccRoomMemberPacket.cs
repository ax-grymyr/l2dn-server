using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExManageMpccRoomMemberPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly MatchingMemberType _memberType;
    private readonly ExManagePartyRoomMemberType _type;
	
    public ExManageMpccRoomMemberPacket(Player player, CommandChannelMatchingRoom room, ExManagePartyRoomMemberType mode)
    {
        _player = player;
        _memberType = room.getMemberType(player);
        _type = mode;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MANAGE_PARTY_ROOM_MEMBER);
        
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_player.ObjectId);
        writer.WriteString(_player.getName());
        writer.WriteInt32((int)_player.getClassId());
        writer.WriteInt32(_player.getLevel());
        writer.WriteInt32(MapRegionManager.getInstance().getBBs(_player.Location.Location2D));
        writer.WriteInt32((int)_memberType);
    }
}