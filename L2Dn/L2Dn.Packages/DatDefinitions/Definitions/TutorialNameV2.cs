using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Salvation, Chronicles.Latest)]
public sealed class TutorialNameV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public TutorialNameRecord[] Records { get; set; } = Array.Empty<TutorialNameRecord>();

    public sealed class TutorialNameRecord
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public uint Type { get; set; }
        public uint Category { get; set; }
        public uint OrderId { get; set; }
        public uint Level { get; set; }
        public string Description { get; set; } = string.Empty;
        public uint DisplayType { get; set; }
    }
}