using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ValidateLocationPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly Location _loc;
	
    public ValidateLocationPacket(WorldObject obj)
    {
        _objectId = obj.getObjectId();
        _loc = obj.getLocation();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VALIDATE_LOCATION);

        writer.WriteInt32(_objectId);
        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
        writer.WriteInt32(_loc.getHeading());
        writer.WriteByte(0xff); // TODO: Find me!
    }
}