using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct StartRotationPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _degree;
    private readonly int _side;
    private readonly int _speed;
	
    public StartRotationPacket(int objectId, int degree, int side, int speed)
    {
        _objectId = objectId;
        _degree = degree;
        _side = side;
        _speed = speed;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.START_ROTATING);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_degree);
        writer.WriteInt32(_side);
        writer.WriteInt32(_speed);
    }
}