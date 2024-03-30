using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions.Shared;

public sealed class MtxNew2
{
    [ArrayLengthType(ArrayLengthType.Byte)]
    [StringType(StringType.NameDataIndex)]
    public string[] Mesh { get; set; } = Array.Empty<string>();

    [ArrayLengthType(ArrayLengthType.Byte)]
    [StringType(StringType.NameDataIndex)]
    public string[] Text { get; set; } = Array.Empty<string>();
}