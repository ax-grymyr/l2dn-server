using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct StopRotationPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _degree;
    private readonly int _speed;
	
    public StopRotationPacket(int objectId, int degree, int speed)
    {
        _objectId = objectId;
        _degree = degree;
        _speed = speed;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.FINISH_ROTATING);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_degree);
        writer.WriteInt32(_speed);
        writer.WriteInt32(0); // ?
    }
}