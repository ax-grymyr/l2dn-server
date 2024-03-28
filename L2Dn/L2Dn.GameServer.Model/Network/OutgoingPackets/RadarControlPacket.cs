using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RadarControlPacket: IOutgoingPacket
{
    private readonly int _showRadar;
    private readonly int _type;
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
	
    public RadarControlPacket(int showRadar, int type, int x, int y, int z)
    {
        _showRadar = showRadar; // showRader?? 0 = showradar; 1 = delete radar;
        _type = type; // radar type??
        _x = x;
        _y = y;
        _z = z;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.RADAR_CONTROL);
        
        writer.WriteInt32(_showRadar);
        writer.WriteInt32(_type); // maybe type
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
    }
}