using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Lindvior, Chronicles.Helios - 1)]
public sealed class AdditionalEffectV3
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalEffectRecord[] Records { get; set; } = Array.Empty<AdditionalEffectRecord>();

    public sealed class AdditionalEffectRecord
    {
        public uint Id { get; set; }
        public string[] EffectNames { get; set; } = Array.Empty<string>();
        public string[] MeshSocketNames { get; set; } = Array.Empty<string>();
        public float[] EffectScales { get; set; } = Array.Empty<float>();
        public uint UsePawnScale { get; set; }
    }
}