using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GetOnVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatObjId;
    private readonly Location3D _location;
	
    /**
     * @param charObjId
     * @param boatObjId
     * @param pos
     */
    public GetOnVehiclePacket(int charObjId, int boatObjId, Location3D location)
    {
        _objectId = charObjId;
        _boatObjId = boatObjId;
        _location = location;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GET_ON_VEHICLE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatObjId);
        writer.WriteLocation3D(_location);
    }
}