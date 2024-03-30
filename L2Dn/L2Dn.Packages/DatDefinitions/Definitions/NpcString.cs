using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.HighFive, Chronicles.Latest)]
public sealed class NpcString
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public NpcStringRecord[] Records { get; set; } = Array.Empty<NpcStringRecord>();

    public sealed class NpcStringRecord
    {
        public uint Id { get; set; }
        public string String { get; set; } = string.Empty;
    }
}