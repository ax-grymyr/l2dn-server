﻿using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.TheSourceOfFlame, Chronicles.Latest)]
public sealed class CombinationItemDataV7
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public CombinationItemDataRecord[] Records { get; set; } = Array.Empty<CombinationItemDataRecord>();

    public sealed class CombinationItemDataRecord
    {
        public ushort Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public byte MaxLevel { get; set; }
        public byte Unknown4 { get; set; }
        public uint Slot1ItemId { get; set; }
        public uint Slot1ItemEnchant { get; set; }
        public uint Slot2ItemId { get; set; }
        public uint Slot2ItemEnchant { get; set; }
        public ResultItem[] ResultItems { get; set; } = Array.Empty<ResultItem>();
        public byte ResultEffectType { get; set; }
        public LocalizationType[] ApplyCountry { get; set; } = Array.Empty<LocalizationType>();
        public long Commission { get; set; }
    }

    public sealed class ResultItem
    {
        public uint ItemId { get; set; }
        public uint ItemEnchant { get; set; }
        public uint Count { get; set; }
        public uint Success { get; set; }
        public uint Chance { get; set; }
    }
}