using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Epilogue - 1)]
public sealed class EtcItemGrp
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public EtcItemGrpRecord[] Records { get; set; } = Array.Empty<EtcItemGrpRecord>();

    public sealed class EtcItemGrpRecord
    {
        public uint Tag { get; set; }
        public uint ObjectId { get; set; }
        public uint DropType { get; set; }
        public uint DropAnimType { get; set; }
        public uint DropRadius { get; set; }
        public uint DropHeight { get; set; }
        public uint Unknown1 { get; set; }

        [ArrayLengthType(ArrayLengthType.Fixed, 3)]
        [StringType(StringType.Utf16Le)]
        public string[] DropMeshes { get; set; } = Array.Empty<string>();
        
        [ArrayLengthType(ArrayLengthType.Fixed, 3)]
        [StringType(StringType.Utf16Le)]
        public string[] DropTextures { get; set; } = Array.Empty<string>();
        
        [ArrayLengthType(ArrayLengthType.Fixed, 5)]
        [StringType(StringType.Utf16Le)]
        public string[] Icons { get; set; } = Array.Empty<string>();

        public uint Durability { get; set; }
        public uint Weight { get; set; }
        public uint MaterialType { get; set; }
        public uint Crystallizable { get; set; }
        public uint Type1 { get; set; }
        public Mtx MeshTexPair { get; set; } = new();
        
        [StringType(StringType.Utf16Le)]
        public string DropSound { get; set; } = string.Empty;
        
        [StringType(StringType.Utf16Le)]
        public string EquipSound { get; set; } = string.Empty;

        public uint Stackable { get; set; }
        public uint EtcItemType { get; set; }
        public uint CrystalType { get; set; }
    }
}