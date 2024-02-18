using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SetupGaugePacket: IOutgoingPacket
{
    public const int BLUE = 0; // TODO: enum
    public const int RED = 1;
    public const int CYAN = 2;
    public const int GREEN = 3;
	
    private readonly int _objectId;
    private readonly int _color;
    private readonly int _currentTime;
    private readonly int _maxTime;
	
    public SetupGaugePacket(int objectId, int color, int time)
    {
        _objectId = objectId;
        _color = color; // color 0-blue 1-red 2-cyan 3-green
        _currentTime = time;
        _maxTime = time;
    }
	
    public SetupGaugePacket(int objectId, int color, int currentTime, int maxTime)
    {
        _objectId = objectId;
        _color = color; // color 0-blue 1-red 2-cyan 3-green
        _currentTime = currentTime;
        _maxTime = maxTime;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SETUP_GAUGE);
        
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_color);
        writer.WriteInt32(_currentTime);
        writer.WriteInt32(_maxTime);
    }
}