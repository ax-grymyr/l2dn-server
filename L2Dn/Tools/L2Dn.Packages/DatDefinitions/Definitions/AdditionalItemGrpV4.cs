using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.Latest)]
public sealed class AdditionalItemGrpV4
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalItemGrpRecord[] Records { get; set; } = Array.Empty<AdditionalItemGrpRecord>();

    public sealed class AdditionalItemGrpRecord
    {
        public uint Id { get; set; }
        public byte HasAny { get; set; }
        public uint[] IncludeItem { get; set; } = Array.Empty<uint>();
        
        public uint MaxEnergy { get; set; }
        public uint LookChange { get; set; }
        public byte CloakHide { get; set; }
        public byte ClockMeshType { get; set; }
        public byte ArmorHide { get; set; }
    }
}