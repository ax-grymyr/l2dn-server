using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Awakening, Chronicles.Lindvior - 1)]
public sealed class ArmorGrpV3
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
        [StringType(StringType.Utf16Le)]
        public string[] DropMesh { get; set; } = Array.Empty<string>();
	
        [ArrayLengthType(ArrayLengthType.Fixed, 4)]
        [StringType(StringType.Utf16Le)]
        public string[] DropTexture { get; set; } = Array.Empty<string>();
	
        [ArrayLengthType(ArrayLengthType.Fixed, 8)]
        public uint[] NewData { get; set; } = Array.Empty<uint>();
	
        [ArrayLengthType(ArrayLengthType.Fixed, 5)]
        [StringType(StringType.Utf16Le)]
        public string[] Icons { get; set; } = Array.Empty<string>();

        public int Durability { get; set; }
        public uint Weight { get; set; }
        public uint MaterialType { get; set; }
        public uint Crystallizable { get; set; }
        public uint Unknown2 { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public uint[] RelatedQuestIds { get; set; } = Array.Empty<uint>();
        
        public uint Color1 { get; set; }
        public uint Color2 { get; set; }
        public uint Color3 { get; set; }

        [StringType(StringType.Utf16Le)]
        public string IconPanel { get; set; } = string.Empty;

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

        public Mtx Npc { get; set; } = new Mtx();
        public Mtx3 NpcAdd { get; set; } = new Mtx3();

        [StringType(StringType.Utf16Le)]
        public string AttackEffect { get; set; } = string.Empty;

        [ArrayLengthType(ArrayLengthType.Int32)]
        [StringType(StringType.Utf16Le)]
        public string[] Sounds { get; set; } = Array.Empty<string>();

        [StringType(StringType.Utf16Le)]
        public string DropSound { get; set; } = string.Empty;

        [StringType(StringType.Utf16Le)]
        public string EquipSound { get; set; } = string.Empty;

        public uint Unknown3 { get; set; }
        public uint Unknown4 { get; set; }
        public uint ArmorType { get; set; }
        public uint CrystalType { get; set; }
        public uint AvoidMod { get; set; }
        public uint Unknown5 { get; set; }
    }
}