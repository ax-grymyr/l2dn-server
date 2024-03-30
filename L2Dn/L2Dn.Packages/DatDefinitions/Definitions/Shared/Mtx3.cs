using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions.Shared;

public sealed class Mtx3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public Mtx3Mesh[] Mesh { get; set; } = Array.Empty<Mtx3Mesh>();

    [ArrayLengthType(ArrayLengthType.Int32)]
    [StringType(StringType.Utf16)]
    public string[] Text { get; set; } = Array.Empty<string>();

    [StringType(StringType.Utf16)]
    public string Text2 { get; set; } = string.Empty;

    public sealed class Mtx3Mesh
    {
        [StringType(StringType.Utf16)]
        public string Mesh { get; set; } = string.Empty;
        
        public byte Val1 { get; set; }
        public byte Val2 { get; set; }
    }
}