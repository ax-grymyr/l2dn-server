using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt, Chronicles.Latest)]
public sealed class CommonLook
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CommonLookRecord[] Records { get; set; } = Array.Empty<CommonLookRecord>();

    public sealed class CommonLookRecord
    {
        public ushort ItemType { get; set; }
        public uint ItemId { get; set; }
    }
}