using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Fishing;

public readonly struct ExUserInfoFishingPacket(int playerObjectId, bool isFishing, Location3D baitLocation = default)
    : IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_INFO_FISHING);
        
        writer.WriteInt32(playerObjectId);
        writer.WriteByte(isFishing);
        writer.WriteLocation3D(baitLocation);
    }
}