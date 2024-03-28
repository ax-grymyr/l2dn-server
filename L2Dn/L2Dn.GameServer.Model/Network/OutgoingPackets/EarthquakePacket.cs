using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct EarthquakePacket: IOutgoingPacket
{
    private readonly int _x;
    private readonly int _y;
    private readonly int _z;
    private readonly int _intensity;
    private readonly int _duration;
	
    /**
     * @param location
     * @param intensity
     * @param duration
     */
    public EarthquakePacket(ILocational location, int intensity, int duration)
    {
        _x = location.getX();
        _y = location.getY();
        _z = location.getZ();
        _intensity = intensity;
        _duration = duration;
    }
	
    /**
     * @param x
     * @param y
     * @param z
     * @param intensity
     * @param duration
     */
    public EarthquakePacket(int x, int y, int z, int intensity, int duration)
    {
        _x = x;
        _y = y;
        _z = z;
        _intensity = intensity;
        _duration = duration;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EARTHQUAKE);
        
        writer.WriteInt32(_x);
        writer.WriteInt32(_y);
        writer.WriteInt32(_z);
        writer.WriteInt32(_intensity);
        writer.WriteInt32(_duration);
        writer.WriteInt32(0); // Unknown
    }
}