using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.Salvation - 1)]
public sealed class TutorialName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public TutorialNameRecord[] Records { get; set; } = Array.Empty<TutorialNameRecord>();

    public sealed class TutorialNameRecord
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public uint Type { get; set; }
    }
}