using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Awakening, Chronicles.Lindvior - 1)]
public sealed class AdditionalEffectV2
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AdditionalEffectRecord[] Records { get; set; } = Array.Empty<AdditionalEffectRecord>();

    public sealed class AdditionalEffectRecord
    {
        public uint Id { get; set; }
        public string EffectName { get; set; } = string.Empty;
        public string MeshSocketName { get; set; } = string.Empty;
    }
}