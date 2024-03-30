using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions.Shared;

public sealed class Mtx3New2
{
    [ArrayLengthType(ArrayLengthType.Byte)]
    [StringType(StringType.NameDataIndex)]
    public string[] Mesh { get; set; } = Array.Empty<string>();

    [ArrayLengthType(ArrayLengthType.PropertyRef, ArrayPropertyName = nameof(Mesh))]
    public ushort[] Mesh2 { get; set; } = Array.Empty<ushort>();

    [ArrayLengthType(ArrayLengthType.Byte)]
    [StringType(StringType.NameDataIndex)]
    public string[] Text { get; set; } = Array.Empty<string>();

    [StringType(StringType.NameDataIndex)]
    public string Text2 { get; set; } = string.Empty;
}