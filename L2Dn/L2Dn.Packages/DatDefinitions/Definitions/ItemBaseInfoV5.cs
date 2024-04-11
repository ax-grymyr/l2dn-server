﻿using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.MasterClass3, Chronicles.Latest)]
public sealed class ItemBaseInfoV5
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ItemBaseInfoRecord[] Records { get; set; } = [];

    public sealed class ItemBaseInfoRecord
    {
        public uint ItemId { get; set; }
        public long DefaultPrice { get; set; }
        public uint GrindPoint { get; set; }
        public uint GrindCommission { get; set; }
        public byte IsLocked { get; set; }
        public byte EnchantEnabled { get; set; }
        public byte EnchantGroupId { get; set; }
        public byte ChallengePointGroupId { get; set; }
        public ushort HeroBookPoints { get; set; }
    }
}