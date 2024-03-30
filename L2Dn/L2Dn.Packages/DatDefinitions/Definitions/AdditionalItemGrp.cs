using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.HighFive, Chronicles.Lindvior - 1)]
public sealed class AdditionalItemGrp
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalItemGrpRecord[] Records { get; set; } = Array.Empty<AdditionalItemGrpRecord>();

    public sealed class AdditionalItemGrpRecord
    {
        public uint Id { get; set; }
        public uint HasAny { get; set; }

        [ArrayLengthType(ArrayLengthType.Fixed, 11)]
        public uint[] Unknown { get; set; } = Array.Empty<uint>();
        
        public uint MaxEnergy { get; set; }
    }
}