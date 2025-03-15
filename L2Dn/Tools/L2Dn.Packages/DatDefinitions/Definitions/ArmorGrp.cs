using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.ScionsOfDestiny, Chronicles.Epilogue - 1)]
public sealed class ArmorGrp
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
	
	    [StringType(StringType.Utf16Le)]
	    public string DropMesh1 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string DropMesh2 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string DropMesh3 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string DropTexture1 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string DropTexture2 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string DropTexture3 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string Icon1 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string Icon2 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string Icon3 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string Icon4 { get; set; } = string.Empty;
	
	    [StringType(StringType.Utf16Le)]
	    public string Icon5 { get; set; } = string.Empty;

	    public int Durability { get; set; }
	    public uint Weight { get; set; }
	    public uint MaterialType { get; set; }
	    public uint Crystallizable { get; set; }
	    public uint Unknown2 { get; set; }
	    public uint BodyPart { get; set; }

	    public Mtx MHumanFighter { get; set; } = new Mtx();
	    public Mtx MHumanFighterAdd { get; set; } = new Mtx();
	    public Mtx FHumanFighter { get; set; } = new Mtx();
	    public Mtx FHumanFighterAdd { get; set; } = new Mtx();

	    public Mtx MDarkElf { get; set; } = new Mtx();
	    public Mtx MDarkElfAdd { get; set; } = new Mtx();
	    public Mtx FDarkElf { get; set; } = new Mtx();
	    public Mtx FDarkElfAdd { get; set; } = new Mtx();

	    public Mtx MDorf { get; set; } = new Mtx();
	    public Mtx MDorfAdd { get; set; } = new Mtx();
	    public Mtx FDorf { get; set; } = new Mtx();
	    public Mtx FDorfAdd { get; set; } = new Mtx();

	    public Mtx MElf { get; set; } = new Mtx();
	    public Mtx MElfAdd { get; set; } = new Mtx();
	    public Mtx FElf { get; set; } = new Mtx();
	    public Mtx FElfAdd { get; set; } = new Mtx();

	    public Mtx MHumanMystic { get; set; } = new Mtx();
	    public Mtx MHumanMysticAdd { get; set; } = new Mtx();
	    public Mtx FHumanMystic { get; set; } = new Mtx();
	    public Mtx FHumanMysticAdd { get; set; } = new Mtx();

	    public Mtx MOrcFighter { get; set; } = new Mtx();
	    public Mtx MOrcFighterAdd { get; set; } = new Mtx();
	    public Mtx FOrcFighter { get; set; } = new Mtx();
	    public Mtx FOrcFighterAdd { get; set; } = new Mtx();

	    public Mtx MOrcMage { get; set; } = new Mtx();
	    public Mtx MOrcMageAdd { get; set; } = new Mtx();
	    public Mtx FOrcMage { get; set; } = new Mtx();
	    public Mtx FOrcMageAdd { get; set; } = new Mtx();

	    public Mtx Unknown3 { get; set; } = new Mtx();

	    public Mtx Npc { get; set; } = new Mtx();
	    public Mtx Aac { get; set; } = new Mtx();

	    [StringType(StringType.Utf16Le)]
	    public string AttackEffect { get; set; } = string.Empty;

	    [ArrayLengthType(ArrayLengthType.Int32)]
	    [StringType(StringType.Utf16Le)]
	    public string[] Sounds { get; set; } = Array.Empty<string>();

	    [StringType(StringType.Utf16Le)]
	    public string DropSound { get; set; } = string.Empty;

	    [StringType(StringType.Utf16Le)]
	    public string EquipSound { get; set; } = string.Empty;

	    public uint Unknown4 { get; set; }
	    public uint Unknown5 { get; set; }
	    public uint ArmorType { get; set; }
	    public uint CrystalType { get; set; }
	    public uint AvoidMod { get; set; }
	    public uint PDef { get; set; }
	    public uint MDef { get; set; }
	    public uint MpBonus { get; set; }
    }
}