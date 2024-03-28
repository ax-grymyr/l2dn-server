using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GetOffVehiclePacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _boatObjId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    /**
     * @param charObjId
     * @param boatObjId
     * @param x
     * @param y
     * @param z
     */
    public GetOffVehiclePacket(int charObjId, int boatObjId, int x, int y, int z)
    {
        _objectId = charObjId;
        _boatObjId = boatObjId;
        _x = x;
        _y = y;
        _z = z;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GET_OFF_VEHICLE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_boatObjId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}