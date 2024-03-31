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
        public IndexedString[] EffectNames { get; set; } = Array.Empty<IndexedString>();
        public IndexedString[] MeshSocketNames { get; set; } = Array.Empty<IndexedString>();
        public IndexedString[] DropMeshSocketNames { get; set; } = Array.Empty<IndexedString>();
        public float[] EffectScales { get; set; } = Array.Empty<float>();
        public uint UsePawnScale { get; set; }
        public uint UseDropEffect { get; set; }
    }
}