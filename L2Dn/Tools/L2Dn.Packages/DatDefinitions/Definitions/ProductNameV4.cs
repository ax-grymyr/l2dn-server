using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.Latest)]
public sealed class ProductNameV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ProductNameRecord[] Records { get; set; } = [];

    public sealed class ProductNameRecord
    {
        public uint Id { get; set; }
        public IndexedString OuterName { get; set; }
        public string Description { get; set; } = string.Empty;
        public IndexedString Icon { get; set; }
        public IndexedString IconPanel { get; set; }
        public string MainSubject { get; set; } = string.Empty;
    }
}