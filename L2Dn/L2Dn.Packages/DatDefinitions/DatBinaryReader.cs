using L2Dn.Packages.Unreal;

namespace L2Dn.Packages.DatDefinitions;

public class DatBinaryReader(Stream stream, long position = 0): UBinaryReader(stream, position)
{
    public string ReadDatString()
    {
        return ReadUString();
    }
}