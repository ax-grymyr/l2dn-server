using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions.Shared;

public sealed class Mtx3New2
{
    [ArrayLengthType(ArrayLengthType.Byte)]
    public IndexedString[] Mesh { get; set; } = Array.Empty<IndexedString>();

    [ArrayLengthType(ArrayLengthType.PropertyRef, ArrayPropertyName = nameof(Mesh))]
    public ushort[] Mesh2 { get; set; } = Array.Empty<ushort>();

    [ArrayLengthType(ArrayLengthType.Byte)]
    public IndexedString[] Text { get; set; } = Array.Empty<IndexedString>();

    public IndexedString Text2 { get; set; }
}