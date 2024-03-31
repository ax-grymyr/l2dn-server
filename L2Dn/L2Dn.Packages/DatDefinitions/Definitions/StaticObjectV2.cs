using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.Latest)]
public sealed class StaticObjectV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public StaticObjectRecord[] Records { get; set; } = Array.Empty<StaticObjectRecord>();

    public sealed class StaticObjectRecord
    {
        public uint Id { get; set; }
        public IndexedString Name { get; set; }
    }
}