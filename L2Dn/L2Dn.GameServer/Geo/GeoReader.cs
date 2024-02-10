using L2Dn.Conversion;
using L2Dn.IO;

namespace L2Dn.GameServer.Geo;

internal sealed class GeoReader(Stream stream, long position = 0)
    : BinaryReader<LittleEndianBitConverter>(stream, position)
{
}