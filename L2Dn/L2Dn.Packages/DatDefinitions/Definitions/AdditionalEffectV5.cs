using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Homunculus, Chronicles.Homunculus2 - 1)]
public sealed class AdditionalEffectV5
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalEffectRecord[] Records { get; set; } = Array.Empty<AdditionalEffectRecord>();

    public sealed class AdditionalEffectRecord
    {
        public uint Id { get; set; }
        
        [StringType(StringType.NameDataIndex)]
        public string[] EffectNames { get; set; } = Array.Empty<string>();

        [StringType(StringType.NameDataIndex)]
        public string[] MeshSocketNames { get; set; } = Array.Empty<string>();

        [StringType(StringType.NameDataIndex)]
        public string[] DropMeshSocketNames { get; set; } = Array.Empty<string>();
        
        public float[] EffectScales { get; set; } = Array.Empty<float>();
        public uint UsePawnScale { get; set; }
        public uint UseDropEffect { get; set; }
    }
}