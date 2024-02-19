using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ExTeleportToLocationActivatePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly Location _loc;
	
    public ExTeleportToLocationActivatePacket(Creature creature)
    {
        _objectId = creature.getObjectId();
        _loc = creature.getLocation();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TELEPORT_TO_LOCATION_ACTIVATE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_loc.getX());
        writer.WriteInt32(_loc.getY());
        writer.WriteInt32(_loc.getZ());
        writer.WriteInt32(0); // Unknown (this isn't instanceId)
        writer.WriteInt32(_loc.getHeading());
        writer.WriteInt32(0); // Unknown
    }
}