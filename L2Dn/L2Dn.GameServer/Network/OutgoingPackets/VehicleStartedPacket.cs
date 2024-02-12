using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct VehicleStartedPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _state;
	
    /**
     * @param boat
     * @param state
     */
    public VehicleStartedPacket(int objectId, int state)
    {
        _objectId = objectId;
        _state = state;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.VEHICLE_START);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_state);
    }
}