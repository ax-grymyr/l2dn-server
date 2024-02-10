namespace L2Dn.GameServer.Model.Geo.Internal;

internal readonly struct GeoCell(short value)
{
    public Direction Nswe => (Direction)(value & 0xF);
    public int Height => (value & 0x0FFF0) >> 1;
}