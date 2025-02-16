using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct ExTeleportToLocationActivatePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly Location _loc;

    public ExTeleportToLocationActivatePacket(Creature creature)
    {
        _objectId = creature.ObjectId;
        _loc = creature.Location;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TELEPORT_TO_LOCATION_ACTIVATE);
        
        writer.WriteInt32(_objectId);
        writer.WriteLocation3D(_loc.Location3D);
        writer.WriteInt32(0); // Unknown (this isn't instanceId)
        writer.WriteInt32(_loc.Heading);
        writer.WriteInt32(0); // Unknown
    }
}