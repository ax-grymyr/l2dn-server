using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TargetSelectedPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _targetObjId;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    /**
     * @param objectId
     * @param targetId
     * @param x
     * @param y
     * @param z
     */
    public TargetSelectedPacket(int objectId, int targetId, int x, int y, int z)
    {
        _objectId = objectId;
        _targetObjId = targetId;
        _x = x;
        _y = y;
        _z = z;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TARGET_SELECTED);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_targetObjId);
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(0); // ?
    }
}