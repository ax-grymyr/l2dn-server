using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Helios - 1)]
public sealed class StaticObject
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public StaticObjectRecord[] Records { get; set; } = Array.Empty<StaticObjectRecord>();

    public sealed class StaticObjectRecord
    {
        public uint Id { get; set; }
        
        [StringType(StringType.Utf16Le)]
        public string Name { get; set; } = string.Empty;
    }
}