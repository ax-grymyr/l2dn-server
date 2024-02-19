using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct TeleportToLocationPacket: IOutgoingPacket
{
    private readonly int _targetObjId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _heading;
	
    public TeleportToLocationPacket(WorldObject obj, int x, int y, int z, int heading)
    {
        _targetObjId = obj.getObjectId();
        _x = x;
        _y = y;
        _z = z;
        _heading = heading;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TELEPORT_TO_LOCATION);
        
        writer.WriteInt32(_targetObjId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(0); // Fade 0, Instant 1.
        writer.WriteInt32(_heading);
        writer.WriteInt32(0); // Unknown.
    }
}