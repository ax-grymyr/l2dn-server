using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt, Chronicles.MasterClass2 - 1)]
public sealed class TeleportListV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public TeleportListRecord[] Records { get; set; } = Array.Empty<TeleportListRecord>();

    public sealed class TeleportListRecord
    {
        public uint HuntingZoneId { get; set; }
        public uint TownId { get; set; }
        public uint Priority { get; set; }
        public TeleportPrice[] Prices { get; set; } = Array.Empty<TeleportPrice>();
        public uint UsableLevel { get; set; }
        public uint UsableTransferDegree { get; set; }
    }

    public sealed class TeleportPrice
    {
        public uint ItemId { get; set; }
        public uint ItemCount { get; set; }
    }
}