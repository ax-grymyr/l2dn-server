using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Matching;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPartyRoomMemberPacket: IOutgoingPacket
{
    private readonly PartyMatchingRoom _room;
    private readonly MatchingMemberType _type;

    public ExPartyRoomMemberPacket(Player player, PartyMatchingRoom room)
    {
        _room = room;
        _type = room.getMemberType(player);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PARTY_ROOM_MEMBER);

        writer.WriteInt32((int)_type);
        writer.WriteInt32(_room.getMembersCount());
        foreach (Player member in _room.getMembers())
        {
            writer.WriteInt32(member.ObjectId);
            writer.WriteString(member.getName());
            writer.WriteInt32((int)member.getActiveClass());
            writer.WriteInt32(member.getLevel());
            writer.WriteInt32(MapRegionData.Instance.GetBBs(member.Location.Location2D));
            writer.WriteInt32((int)_room.getMemberType(member));
            Map<int, DateTime> instanceTimes = InstanceManager.getInstance().getAllInstanceTimes(member);
            writer.WriteInt32(instanceTimes.Count);

            DateTime now = DateTime.UtcNow;
            foreach (var entry in instanceTimes)
            {
                int instanceTime = (int)(entry.Value - now).TotalSeconds;
                writer.WriteInt32(entry.Key);
                writer.WriteInt32(instanceTime);
            }
        }
    }
}