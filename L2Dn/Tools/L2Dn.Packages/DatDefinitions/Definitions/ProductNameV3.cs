using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Underground, Chronicles.Helios - 1)]
public sealed class ProductNameV3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ProductNameRecord[] Records { get; set; } = [];

    public sealed class ProductNameRecord
    {
        public uint Id { get; set; }
        
        [StringType(StringType.Utf16Le)] 
        public string OuterName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringType(StringType.Utf16Le)]
        public string Icon { get; set; } = string.Empty;

        [StringType(StringType.Utf16Le)]
        public string IconPanel { get; set; } = string.Empty;

        public string MainSubject { get; set; } = string.Empty;
    }
}