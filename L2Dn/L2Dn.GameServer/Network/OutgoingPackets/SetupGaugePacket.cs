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
    private readonly TimeSpan _currentTime;
    private readonly TimeSpan _maxTime;
	
    public SetupGaugePacket(int objectId, int color, TimeSpan time)
    {
        _objectId = objectId;
        _color = color; // color 0-blue 1-red 2-cyan 3-green
        _currentTime = time;
        _maxTime = time;
    }
	
    public SetupGaugePacket(int objectId, int color, TimeSpan currentTime, TimeSpan maxTime)
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
        writer.WriteInt32((int)_currentTime.TotalMilliseconds); // TODO calculate via ticks
        writer.WriteInt32((int)_maxTime.TotalMilliseconds);
    }
}