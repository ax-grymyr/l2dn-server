using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions.Shared;

public sealed class MtxNew2
{
    [ArrayLengthType(ArrayLengthType.Byte)]
    public IndexedString[] Mesh { get; set; } = Array.Empty<IndexedString>();

    [ArrayLengthType(ArrayLengthType.Byte)]
    public IndexedString[] Text { get; set; } = Array.Empty<IndexedString>();
}