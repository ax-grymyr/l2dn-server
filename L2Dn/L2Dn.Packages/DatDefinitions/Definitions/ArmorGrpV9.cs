using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ReturnOfTheQueenAnt, Chronicles.ReturnOfTheQueenAnt2 - 1)]
public sealed class ArmorGrpV9
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ArmorGrpRecord[] Records { get; set; } = Array.Empty<ArmorGrpRecord>();

    public sealed class ArmorGrpRecord
    {
        public byte Tag { get; set; }
        public uint ObjectId { get; set; }
        public byte DropType { get; set; }
        public byte DropAnimType { get; set; }
        public byte DropRadius { get; set; }
        public byte DropHeight { get; set; }

        [ArrayLengthType(ArrayLengthType.Byte)]
        public DropTexture[] DropTextures { get; set; } = Array.Empty<DropTexture>();
	
        [ArrayLengthType(ArrayLengthType.Fixed, 5)]
        [StringType(StringType.NameDataIndex)]
        public string[] Icons { get; set; } = Array.Empty<string>();

        public short Durability { get; set; }
        public short Weight { get; set; }
        public byte MaterialType { get; set; } // enum material_type
        public byte Crystallizable { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Byte)]
        public short[] RelatedQuestIds { get; set; } = Array.Empty<short>();
        
        public byte Color { get; set; }
        public byte IsAttribution { get; set; }
        public short PropertyParams { get; set; }

        [StringType(StringType.NameDataIndex)]
        public string IconPanel { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)]
        public string CompleteItemDropSoundType { get; set; } = string.Empty;

        public byte InventoryType { get; set; } // enum inventory_type
        public byte BodyPart { get; set; } // enum bodypart_v2_type

        public MtxNew2 MHumanFighter { get; set; } = new MtxNew2();
        public Mtx3New2 MHumanFighterAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FHumanFighter { get; set; } = new MtxNew2();
        public Mtx3New2 FHumanFighterAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MDarkElf { get; set; } = new MtxNew2();
        public Mtx3New2 MDarkElfAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FDarkElf { get; set; } = new MtxNew2();
        public Mtx3New2 FDarkElfAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MDorf { get; set; } = new MtxNew2();
        public Mtx3New2 MDorfAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FDorf { get; set; } = new MtxNew2();
        public Mtx3New2 FDorfAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MElf { get; set; } = new MtxNew2();
        public Mtx3New2 MElfAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FElf { get; set; } = new MtxNew2();
        public Mtx3New2 FElfAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MHumanMystic { get; set; } = new MtxNew2();
        public Mtx3New2 MHumanMysticAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FHumanMystic { get; set; } = new MtxNew2();
        public Mtx3New2 FHumanMysticAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MOrcFighter { get; set; } = new MtxNew2();
        public Mtx3New2 MOrcFighterAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FOrcFighter { get; set; } = new MtxNew2();
        public Mtx3New2 FOrcFighterAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MOrcMage { get; set; } = new MtxNew2();
        public Mtx3New2 MOrcMageAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FOrcMage { get; set; } = new MtxNew2();
        public Mtx3New2 FOrcMageAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MKamael { get; set; } = new MtxNew2();
        public Mtx3New2 MKamaelAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FKamael { get; set; } = new MtxNew2();
        public Mtx3New2 FKamaelAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MErtheia { get; set; } = new MtxNew2();
        public Mtx3New2 MErtheiaAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FErtheia { get; set; } = new MtxNew2();
        public Mtx3New2 FErtheiaAdd { get; set; } = new Mtx3New2();

        public MtxNew2 MSylph { get; set; } = new MtxNew2();
        public Mtx3New2 MSylphAdd { get; set; } = new Mtx3New2();
        public MtxNew2 FSylph { get; set; } = new MtxNew2();
        public Mtx3New2 FSylphAdd { get; set; } = new Mtx3New2();

        public MtxNew2 Npc { get; set; } = new MtxNew2();
        public Mtx3New2 NpcAdd { get; set; } = new Mtx3New2();

        [StringType(StringType.NameDataIndex)]
        public string AttackEffect { get; set; } = string.Empty;

        [ArrayLengthType(ArrayLengthType.Byte)]
        [StringType(StringType.NameDataIndex)]
        public string[] Sounds { get; set; } = Array.Empty<string>();

        [StringType(StringType.NameDataIndex)]
        public string DropSound { get; set; } = string.Empty;

        [StringType(StringType.NameDataIndex)]
        public string EquipSound { get; set; } = string.Empty;

        public uint Unknown6 { get; set; }
        public byte Unknown7 { get; set; }
        public byte ArmorType { get; set; } // enum armor_type
        public byte CrystalType { get; set; } // enum grade_type
        public short MpBonus { get; set; }
        public short HideMask { get; set; }
        public byte UnderwearBodyPart1 { get; set; }
        public byte UnderwearBodyPart2 { get; set; }
        public byte FullArmorEnchantEffectType { get; set; }
    }

    public sealed class DropTexture
    {
        [StringType(StringType.NameDataIndex)]
        public string DropMesh { get; set; } = string.Empty;

        [ArrayLengthType(ArrayLengthType.Byte)]
        [StringType(StringType.NameDataIndex)]
        public string[] DropTexture2 { get; set; } = Array.Empty<string>();
    }
}