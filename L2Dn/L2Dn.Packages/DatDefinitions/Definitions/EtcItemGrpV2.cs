using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Epilogue, Chronicles.Awakening - 1)]
public sealed class EtcItemGrpV2
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
        
        [ArrayLengthType(ArrayLengthType.Fixed, 4)]
        [StringType(StringType.Utf16Le)]
        public string[] DropTextures { get; set; } = Array.Empty<string>();
        
        [ArrayLengthType(ArrayLengthType.Fixed, 8)]
        public uint[] NewData { get; set; } = Array.Empty<uint>();
        
        [ArrayLengthType(ArrayLengthType.Fixed, 5)]
        [StringType(StringType.Utf16Le)]
        public string[] Icons { get; set; } = Array.Empty<string>();

        public uint Durability { get; set; }
        public uint Weight { get; set; }
        
        [EnumValueType(EnumValueType.Int32)]
        public MaterialType MaterialType { get; set; }

        public uint Crystallizable { get; set; }
        public RgbaColor Unknown2 { get; set; } = new();
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public uint[] RelatedQuestIds { get; set; } = Array.Empty<uint>();
        
        public uint Color { get; set; }
        
        [StringType(StringType.Utf16Le)]
        public string IconPanel { get; set; } = string.Empty;

        [ArrayLengthType(ArrayLengthType.Int32)]
        [StringType(StringType.Utf16Le)]
        public string[] Meshes { get; set; } = Array.Empty<string>();

        [ArrayLengthType(ArrayLengthType.Int32)]
        [StringType(StringType.Utf16Le)]
        public string[] Textures { get; set; } = Array.Empty<string>();

        [StringType(StringType.Utf16Le)]
        public string DropSound { get; set; } = string.Empty;
        
        [StringType(StringType.Utf16Le)]
        public string EquipSound { get; set; } = string.Empty;

        [EnumValueType(EnumValueType.Int32)]
        public ConsumeType ConsumeType { get; set; }

        public ItemEtcV2Type EtcItemType { get; set; }

        [EnumValueType(EnumValueType.Int32)]
        public GradeType CrystalType { get; set; }
    }
}