using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GetOnVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatObjId;
    private readonly Location _pos;
	
    /**
     * @param charObjId
     * @param boatObjId
     * @param pos
     */
    public GetOnVehiclePacket(int charObjId, int boatObjId, Location pos)
    {
        _objectId = charObjId;
        _boatObjId = boatObjId;
        _pos = pos;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GET_ON_VEHICLE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatObjId);
        writer.WriteInt32(_pos.getX());
        writer.WriteInt32(_pos.getY());
        writer.WriteInt32(_pos.getZ());
    }
}