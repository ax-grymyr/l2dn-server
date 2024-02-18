using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/**
 * @author Maktakien
 */
public readonly struct VehicleCheckLocationPacket: IOutgoingPacket
{
    private readonly Creature _boat;

    public VehicleCheckLocationPacket(Creature boat)
    {
        _boat = boat;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VEHICLE_CHECK_LOCATION);
        
        writer.WriteInt32(_boat.getObjectId());
        writer.WriteInt32(_boat.getX());
        writer.WriteInt32(_boat.getY());
        writer.WriteInt32(_boat.getZ());
        writer.WriteInt32(_boat.getHeading());
    }
}