using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions.Shared;

public sealed class Mtx
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    [StringType(StringType.Utf16)]
    public string[] Mesh { get; set; } = Array.Empty<string>();

    [ArrayLengthType(ArrayLengthType.Int32)]
    [StringType(StringType.Utf16)]
    public string[] Text { get; set; } = Array.Empty<string>();
}