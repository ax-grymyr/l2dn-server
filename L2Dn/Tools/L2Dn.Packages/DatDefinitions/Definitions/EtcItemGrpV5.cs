using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Helios, Chronicles.MasterClass - 1)]
public sealed class EtcItemGrpV5
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public EtcItemGrpRecord[] Records { get; set; } = Array.Empty<EtcItemGrpRecord>();

    public sealed class EtcItemGrpRecord
    {
        public byte Tag { get; set; }
        public uint ObjectId { get; set; }
        public byte DropType { get; set; }
        public byte DropAnimType { get; set; }
        public byte DropRadius { get; set; }
        public byte DropHeight { get; set; }
        public uint Unknown1 { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public DropTexture[] DropTextures { get; set; } = Array.Empty<DropTexture>();

        [ArrayLengthType(ArrayLengthType.Fixed, 5)]
        public IndexedString[] Icons { get; set; } = Array.Empty<IndexedString>();

        public short Durability { get; set; }
        public short Weight { get; set; }
        public MaterialType MaterialType { get; set; }
        public byte Crystallizable { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        public short[] RelatedQuestIds { get; set; } = Array.Empty<short>();
        
        public byte Color { get; set; }
        public byte IsAttribution { get; set; }
        public short PropertyParams { get; set; }
        public IndexedString IconPanel { get; set; }
        public IndexedString CompleteItemDropSoundType { get; set; }
        public InventoryType InventoryType { get; set; }

        // MTX_NEW2
        [ArrayLengthType(ArrayLengthType.Byte)]
        public IndexedString[] Meshes { get; set; } = Array.Empty<IndexedString>(); 

        [ArrayLengthType(ArrayLengthType.Byte)]
        public IndexedString[] Textures { get; set; } = Array.Empty<IndexedString>(); 
        
        public IndexedString DropSound { get; set; }
        public IndexedString EquipSound { get; set; }
        public ConsumeType ConsumeType { get; set; }
        public ItemEtcV2Type EtcItemType { get; set; }
        public GradeType CrystalType { get; set; }
    }

    public sealed class DropTexture
    {
        public IndexedString Mesh { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public IndexedString[] Texture { get; set; } = Array.Empty<IndexedString>();
    }
}