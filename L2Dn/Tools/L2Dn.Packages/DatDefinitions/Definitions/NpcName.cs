using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Latest)]
public sealed class NpcName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public NpcNameRecord[] Records { get; set; } = Array.Empty<NpcNameRecord>();

    public sealed class NpcNameRecord
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Nick { get; set; } = string.Empty;
        public RgbaColor NickColor { get; set; } = new RgbaColor();
    }
}