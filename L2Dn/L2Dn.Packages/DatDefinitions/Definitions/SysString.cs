using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Latest)]
public sealed class SysString
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SysStringRecord[] Records { get; set; } = Array.Empty<SysStringRecord>();

    public sealed class SysStringRecord
    {
        public uint Id { get; set; }
        public string String { get; set; } = string.Empty;
    }
}