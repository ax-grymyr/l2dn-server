using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Underground, Chronicles.Helios - 1)]
public sealed class ArmorGrpV7
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public ArmorGrpRecord[] Records { get; set; } = Array.Empty<ArmorGrpRecord>();

    public sealed class ArmorGrpRecord
    {
        public uint Tag { get; set; }
        public uint ObjectId { get; set; }
        public uint DropType { get; set; }
        public uint DropAnimType { get; set; }
        public uint DropRadius { get; set; }
        public uint DropHeight { get; set; }
        public uint Unknown1 { get; set; }
	
        [ArrayLengthType(ArrayLengthType.Fixed, 3)]
        [StringType(StringType.Utf16)]
        public string[] DropMesh { get; set; } = Array.Empty<string>();
	
        [ArrayLengthType(ArrayLengthType.Fixed, 9)]
        [StringType(StringType.Utf16)]
        public string[] DropTexture { get; set; } = Array.Empty<string>();
	
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        public uint Unknown4 { get; set; }
	
        [ArrayLengthType(ArrayLengthType.Fixed, 5)]
        [StringType(StringType.Utf16)]
        public string[] Icons { get; set; } = Array.Empty<string>();

        public uint Durability { get; set; }
        public uint Weight { get; set; }
        public uint MaterialType { get; set; }
        public uint Crystallizable { get; set; }
        public uint Unknown5 { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public uint[] RelatedQuestIds { get; set; } = Array.Empty<uint>();
        
        public uint Color { get; set; }
        public uint IsAttribution { get; set; }
        public uint PropertyParams { get; set; }

        [StringType(StringType.Utf16)]
        public string IconPanel { get; set; } = string.Empty;

        public string CompleteItemDropSoundType { get; set; } = string.Empty;
        public uint InventoryType { get; set; }
        public uint BodyPart { get; set; }

        public Mtx MHumanFighter { get; set; } = new Mtx();
        public Mtx3 MHumanFighterAdd { get; set; } = new Mtx3();
        public Mtx FHumanFighter { get; set; } = new Mtx();
        public Mtx3 FHumanFighterAdd { get; set; } = new Mtx3();

        public Mtx MDarkElf { get; set; } = new Mtx();
        public Mtx3 MDarkElfAdd { get; set; } = new Mtx3();
        public Mtx FDarkElf { get; set; } = new Mtx();
        public Mtx3 FDarkElfAdd { get; set; } = new Mtx3();

        public Mtx MDorf { get; set; } = new Mtx();
        public Mtx3 MDorfAdd { get; set; } = new Mtx3();
        public Mtx FDorf { get; set; } = new Mtx();
        public Mtx3 FDorfAdd { get; set; } = new Mtx3();

        public Mtx MElf { get; set; } = new Mtx();
        public Mtx3 MElfAdd { get; set; } = new Mtx3();
        public Mtx FElf { get; set; } = new Mtx();
        public Mtx3 FElfAdd { get; set; } = new Mtx3();

        public Mtx MHumanMystic { get; set; } = new Mtx();
        public Mtx3 MHumanMysticAdd { get; set; } = new Mtx3();
        public Mtx FHumanMystic { get; set; } = new Mtx();
        public Mtx3 FHumanMysticAdd { get; set; } = new Mtx3();

        public Mtx MOrcFighter { get; set; } = new Mtx();
        public Mtx3 MOrcFighterAdd { get; set; } = new Mtx3();
        public Mtx FOrcFighter { get; set; } = new Mtx();
        public Mtx3 FOrcFighterAdd { get; set; } = new Mtx3();

        public Mtx MOrcMage { get; set; } = new Mtx();
        public Mtx3 MOrcMageAdd { get; set; } = new Mtx3();
        public Mtx FOrcMage { get; set; } = new Mtx();
        public Mtx3 FOrcMageAdd { get; set; } = new Mtx3();

        public Mtx MKamael { get; set; } = new Mtx();
        public Mtx3 MKamaelAdd { get; set; } = new Mtx3();
        public Mtx FKamael { get; set; } = new Mtx();
        public Mtx3 FKamaelAdd { get; set; } = new Mtx3();

        public Mtx MErtheia { get; set; } = new Mtx();
        public Mtx3 MErtheiaAdd { get; set; } = new Mtx3();
        public Mtx FErtheia { get; set; } = new Mtx();
        public Mtx3 FErtheiaAdd { get; set; } = new Mtx3();

        public Mtx Npc { get; set; } = new Mtx();
        public Mtx3 NpcAdd { get; set; } = new Mtx3();

        [StringType(StringType.Utf16)]
        public string AttackEffect { get; set; } = string.Empty;

        [ArrayLengthType(ArrayLengthType.Int32)]
        [StringType(StringType.Utf16)]
        public string[] Sounds { get; set; } = Array.Empty<string>();

        [StringType(StringType.Utf16)]
        public string DropSound { get; set; } = string.Empty;

        [StringType(StringType.Utf16)]
        public string EquipSound { get; set; } = string.Empty;

        public uint Unknown6 { get; set; }
        public uint Unknown7 { get; set; }
        public uint ArmorType { get; set; }
        public uint CrystalType { get; set; }
        public uint AvoidMod { get; set; }
        public uint Unknown8 { get; set; }
        public uint Unknown9 { get; set; }
        public uint Unknown10 { get; set; }
        public uint Unknown11 { get; set; }
    }
}