using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Enums;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Shinemaker, Chronicles.Latest)]
public sealed class WeaponGrpV12
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public WeaponGrpRecord[] Records { get; set; } = Array.Empty<WeaponGrpRecord>();

    public sealed class WeaponGrpRecord
    {
        public byte Tag { get; set; }
        public uint ObjectId { get; set; }
        public byte DropType { get; set; }
        public byte DropAnimType { get; set; }
        public byte DropRadius { get; set; }
        public byte DropHeight { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        public DropTexture[] Textures { get; set; } = Array.Empty<DropTexture>();

        [ArrayLengthType(ArrayLengthType.Fixed, 5)]
        public IndexedString[] Icons { get; set; } = Array.Empty<IndexedString>();
        
        public short Durability { get; set; }
        public short Weight { get; set; }
        public MaterialType MaterialType { get; set; }
        public byte Crystallizable { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public RelatedQuest[] RelatedQuests { get; set; } = Array.Empty<RelatedQuest>();
        
        public byte Color { get; set; }
        public byte IsAttribution { get; set; }
        public IndexedString IconPanel { get; set; }
        public IndexedString CompleteItemDropSoundType { get; set; }
        public InventoryType InventoryType { get; set; }
        public BodyPartV2Type BodyPart { get; set; }
        public byte Handness { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        public IndexedString[] WpMeshes { get; set; } = Array.Empty<IndexedString>();
        
        [ArrayLengthType(ArrayLengthType.PropertyRef, ArrayPropertyName = nameof(WpMeshes))]
        public byte[] WpMeshes1 { get; set; } = Array.Empty<byte>();
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        public IndexedString[] Textures1 { get; set; } = Array.Empty<IndexedString>();
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        public IndexedString[] ItemSounds { get; set; } = Array.Empty<IndexedString>();

        public IndexedString DropSound { get; set; }
        public IndexedString EquipSound { get; set; }
        public IndexedString Effect { get; set; }

        public byte RandomDamage { get; set; }
        public ItemWeaponV2Type WeaponType { get; set; }
        public GradeType CrystalType { get; set; }
        public byte MpConsume { get; set; }
        public byte SoulshotCount { get; set; }
        public byte SpiritshotCount { get; set; }
        public short Curvature { get; set; }
        public byte Unknown2 { get; set; }
        public byte CanEquipHero { get; set; }
        public byte IsMagicWeapon { get; set; }
        public float ErtheiaFistScale { get; set; }
        public short Junk { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public Enchanted[] Enchants { get; set; } = Array.Empty<Enchanted>();
        
        public byte VariationEffectType1 { get; set; }
        public byte VariationEffectType2 { get; set; }
        public byte VariationEffectType3 { get; set; }
        public byte VariationEffectType4 { get; set; }
        public byte VariationEffectType5 { get; set; }
        public byte VariationEffectType6 { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        public IndexedString[] VariationIcons { get; set; } = Array.Empty<IndexedString>();
        
        public byte NormalEnsoulCount { get; set; }
        public byte SpecialEnsoulCount { get; set; }
    }

    public sealed class DropTexture
    {
        public IndexedString Mesh { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        public IndexedString[] Textures { get; set; } = Array.Empty<IndexedString>();

        public IndexedString Texture2 { get; set; }
        public IndexedString Texture3 { get; set; }
    }

    public sealed class RelatedQuest
    {
        public short QuestId { get; set; }
        public short Unknown { get; set; }
    }

    public sealed class Enchanted
    {
        public IndexedString EnchantEffect { get; set; }
        public float EnchantedEffectOffset1 { get; set; }
        public float EnchantedEffectOffset2 { get; set; }
        public float EnchantedEffectOffset3 { get; set; }
        public float EnchantedMeshOffset1 { get; set; }
        public float EnchantedMeshOffset2 { get; set; }
        public float EnchantedMeshOffset3 { get; set; }
        public float EnchantedMeshScale1 { get; set; }
        public float EnchantedMeshScale2 { get; set; }
        public float EnchantedMeshScale3 { get; set; }
        public float EnchantedEffectVelocity { get; set; }
        public float EnchantedParticleScale { get; set; }
        public float EnchantedEffectScale { get; set; }
        public float EnchantedParticleOffset1 { get; set; }
        public float EnchantedParticleOffset2 { get; set; }
        public float EnchantedParticleOffset3 { get; set; }
        public float EnchantedRingOffset1 { get; set; }
        public float EnchantedRingOffset2 { get; set; }
        public float EnchantedRingOffset3 { get; set; }
        public float EnchantedRingScale1 { get; set; }
        public float EnchantedRingScale2 { get; set; }
        public float EnchantedRingScale3 { get; set; }
    }
}