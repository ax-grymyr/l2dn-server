using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt, Chronicles.Latest)]
public sealed class Subjugation
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public SubjugationRecord[] Records { get; set; } = Array.Empty<SubjugationRecord>();

    public sealed class SubjugationRecord
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Banner { get; set; } = string.Empty;
        public uint MinLevel { get; set; }
        public uint MaxLevel { get; set; }

        public uint MaxSubjugationPoint { get; set; }
        public uint MaxGachaPoint { get; set; }
        
        public uint MaxPeriodicGachaPoint { get; set; }
        public uint GachaCostItem { get; set; }
        public uint GachaCostNum { get; set; }
        public uint MaxUsePoint { get; set; }
        public uint TeleportId { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] Cycles { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] HotTimes { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] ShowGachaMain { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] ShowGachaSub { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] RewardRank1 { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] RewardRank2 { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] RewardRank3 { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] RewardRank4 { get; set; } = Array.Empty<uint>();

        [ArrayLengthType(ArrayLengthType.Byte)]
        public uint[] RewardRank5 { get; set; } = Array.Empty<uint>();
    }
}