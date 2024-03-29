using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.PreludeOfWar, Chronicles.ReturnOfTheQueenAnt - 1)]
public sealed class TeleportList
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public TeleportListRecord[] Records { get; set; } = Array.Empty<TeleportListRecord>();

    public sealed class TeleportListRecord
    {
        public uint HuntingZoneId { get; set; }
        public uint TownId { get; set; }
        public uint Price { get; set; }
        public uint Priority { get; set; }
    }
}