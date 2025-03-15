using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Valiance, Chronicles.Helios - 1)]
public sealed class AdditionalItemGrpV3
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
        public uint LookChange { get; set; }
        public uint CloakHide { get; set; }
        public uint ClockMeshType { get; set; }
        public uint ArmorHide { get; set; }
    }
}