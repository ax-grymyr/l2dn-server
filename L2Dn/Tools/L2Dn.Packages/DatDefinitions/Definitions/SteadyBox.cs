using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Homunculus2, Chronicles.Latest)]
public sealed class SteadyBox
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SteadyBoxRecord[] Records { get; set; } = Array.Empty<SteadyBoxRecord>();

    public sealed class SteadyBoxRecord
    {
        public uint Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}