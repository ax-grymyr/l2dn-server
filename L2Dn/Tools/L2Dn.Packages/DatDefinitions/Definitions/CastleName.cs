using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Epilogue - 1)]
public sealed class CastleName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CastleNameRecord[] Records { get; set; } = Array.Empty<CastleNameRecord>();

    public sealed class CastleNameRecord
    {
        public uint Number { get; set; }
        public uint Tag { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}